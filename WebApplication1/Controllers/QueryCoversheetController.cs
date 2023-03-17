using JobTrack.Models;
using JobTrack.Models.Enums;
using JobTrack.Models.QueryCoversheet;
using JobTrack.Models.QueryManuscript;
using JobTrack.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JobTrack.Controllers
{
    public class QueryCoversheetController : Controller
    {
        private readonly IQueryCoversheetService _queryCoversheetService;

        public QueryCoversheetController(IQueryCoversheetService queryCoversheetService)
        {
            _queryCoversheetService = queryCoversheetService;
        }

        public async Task<ActionResult> Index(int id, int u)
        {
            ViewBag.UserAccess = (UserAccessEnum)u;
            ViewBag.CoversheetID = id;
            var viewModel = await _queryCoversheetService.GetCoversheetDataByIdAsync(id);
            return View(viewModel);
        }   

        public async Task<ActionResult> Reply(int id, int queryid, bool v, int u)
        {
            // relogin for new session
            if (Session["UserName"] == null)
            {
                TempData["alertMessage"] = "You must log in to continue";
                return RedirectToAction("Login", "Login");
            }

            // v (isViewOnly)
            var model = new ReplyModel { QueryID = queryid };
            model.Replies = await _queryCoversheetService.GetCoverRepliesAsync(queryid);

            var querycoversheet = await _queryCoversheetService.GetQueryCoversheetByIdAsync(queryid);
            model.CoverStatusID = querycoversheet.CoverStatusID.Value;
            model.CoverTopicTitle = querycoversheet.CoverTopicTitle;
            model.CoverType = querycoversheet.CoverType;
            model.CoversheetID = id;

            ViewBag.UserAccess = (UserAccessEnum)u;
            ViewBag.UserName = !(Session["UserName"] is null) ? Session["UserName"] : "system";
            ViewBag.IsViewOnly = v;

            return View(model);
        }

        public async Task<ActionResult> _ReplyBox(ReplyModel model)
        {
            return PartialView(await Task.FromResult(model));
        }

        public async Task<string> GetQueryCoversheet(int coversheetId, int filterBy)
        {
            var result = new JsonResultModel();
            var filter = (FilterEnum)filterBy;

            try
            {
                result.Collection = await _queryCoversheetService.GetQueryCoversheetAsync(coversheetId, filter);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return JsonConvert.SerializeObject(result);
        }

        public async Task<ActionResult> UpdateQueryCoversheet(QueryCoversheetModel model)
        {
            var isNew = model.ID < 1;
            var jsonResult = new JsonResultModel();
            var fileResult = new JsonResultModel { IsSuccess = true };
            var currentUser = Session["UserName"] != null ? Session["UserName"].ToString() : "system";

            try
            {
                model.PostedBy = currentUser;

                if (isNew)
                {
                    // save new query and get new ID
                    model.FilePath = string.Empty;  // fix for "Unhandled type encountered"
                    var newData = await _queryCoversheetService.UpdateQueryCoversheetAsync(model);
                    model.ID = newData.ID;

                    // save file to directory
                    if (model.FileToUpload != null)
                    {
                        fileResult = await WriteToFile($@"C:\jobtrackaunz\coversheet\{model.ID}\{model.FileToUpload.FileName}"
                            , $@"C:\jobtrackaunz\coversheet\{model.ID}\"
                            , model.FileToUpload.InputStream);

                        if (fileResult.IsSuccess)
                            model.FilePath = model.FileToUpload != null 
                                ? $@"C:\jobtrackaunz\coversheet\{model.ID}\{model.FileToUpload.FileName}" 
                                : string.Empty;

                        var updatedData = await _queryCoversheetService.UpdateQueryCoversheetAsync(model);
                    }
                }
                else
                {
                    // save file to directory
                    if (model.FileToUpload != null)
                        fileResult = await WriteToFile($@"C:\jobtrackaunz\coversheet\{model.ID}\{model.FileToUpload.FileName}"
                            , $@"C:\jobtrackaunz\coversheet\{model.ID}\"
                            , model.FileToUpload.InputStream);

                    if (fileResult.IsSuccess)
                        model.FilePath = model.FileToUpload != null 
                            ? $@"C:\jobtrackaunz\coversheet\{model.ID}\{model.FileToUpload.FileName}" 
                            : string.Empty;

                    var updatedData = await _queryCoversheetService.UpdateQueryCoversheetAsync(model);
                }

                // save data to "coverreplies" table with "coversheetquery" ID as foreign_key
                jsonResult.IsSuccess = await _queryCoversheetService.UpdateCoverReplyAsync(new ReplyModel
                {
                    QueryID = model.ID,
                    Message = string.IsNullOrEmpty(model.Message) ? string.Empty : model.Message,
                    PostedBy = currentUser
                });

                jsonResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                jsonResult.ErrorMessage = ex.Message;
            }

            return Json(jsonResult);
        }

        public async Task<ActionResult> DeleteQueryCoversheet(QueryCoversheetModel model)
        {
            var isSuccess = await _queryCoversheetService.DeleteQueryCoversheetAsync(model);

            return Json(new { success = isSuccess });
        }

        public async Task<ActionResult> UpdateReply(ReplyModel model)
        {
            model.PostedBy = Session["UserName"] != null ? (string)Session["UserName"] : "system";
            var isSuccess = await _queryCoversheetService.UpdateCoverReplyAsync(model);

            if (isSuccess)
                isSuccess = await _queryCoversheetService.UpdateQueryCoversheetStatusAsync(model);
            
            return Json(new { success = isSuccess });
        }

        private async Task<JsonResultModel> WriteToFile(string destinationFile, string destinationPath, Stream stream)
        {
            var result = new JsonResultModel();

            try
            {
                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }

                var fileStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write);
                stream.Position = 0;
                stream.CopyTo(fileStream);
                fileStream.Dispose();

                result.Collection = destinationFile;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return await Task.FromResult(result);
        }
    }
}