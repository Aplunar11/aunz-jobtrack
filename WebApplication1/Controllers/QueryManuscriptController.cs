using JobTrack.Models;
using JobTrack.Models.Enums;
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
    public class QueryManuscriptController : Controller
    {
        private readonly IQueryManuscriptService _queryManuscriptService;

        public QueryManuscriptController(IQueryManuscriptService queryManuscriptService)
        {
            _queryManuscriptService = queryManuscriptService;
        }

        // GET: QueryManuscript
        public async Task<ActionResult> Index(int id, int u)
        {
            ViewBag.UserAccess = (UserAccessEnum)u;
            ViewBag.ManuscriptID = id;
            var viewModel = await _queryManuscriptService.GetManuscriptDetailsByIdAsync(id);
            return View(viewModel);
        }

        public async Task<ActionResult> Reply(int id, int queryid, bool v, int u)
        {
            // v (isViewOnly)

            var model = new ReplyModel { QueryID = queryid };
            model.Replies = await _queryManuscriptService.GetQueryRepliesAsync(queryid);

            var queryManuscript = await _queryManuscriptService.GetQueryManuscriptByIdAsync(queryid);
            model.QueryStatusID = queryManuscript.QueryStatusID;
            model.QueryTopicTitle = queryManuscript.QueryTopicTitle;
            model.QueryType = queryManuscript.QueryType;
            model.ManuscriptID = id;

            ViewBag.UserAccess = (UserAccessEnum)u;
            ViewBag.IsViewOnly = v;

            return View(model);
        }

        public async Task<ActionResult> _EditQueryManuscript(QueryManuscriptModel model)
        {
            var result = await _queryManuscriptService.GetQueryManuscriptByIdAsync(model.ID);
            if (result != null)
                return PartialView(result);
            
            return PartialView(model);
        }

        public async Task<ActionResult> _ReplyBox(ReplyModel model)
        {
            return PartialView(await Task.FromResult(model));
        }

        public async Task<string> GetQueryManuscript(int jobId, int filterBy)
        {
            var result = new JsonResultModel();
            var filter = (FilterEnum)filterBy;

            try
            {
                result.Collection = await _queryManuscriptService.GetQueryManuscriptAsync(jobId, filter);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return JsonConvert.SerializeObject(result);
        }

        public async Task<ActionResult> UpdateQueryManuscript(QueryManuscriptModel model)
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
                    // save new query
                    model.FilePath = string.Empty;  // fix for "Unhandled type encountered"
                    var newData = await _queryManuscriptService.UpdateQueryManuscriptAsync(model);
                    model.ID = newData.ID;

                    // save file to directory
                    if (model.FileToUpload != null)
                    {
                        fileResult = await WriteToFile($@"C:\jobtrackaunz\manuscript\{model.ID}\{model.FileToUpload.FileName}"
                            , $@"C:\jobtrackaunz\manuscript\{model.ID}\"
                            , model.FileToUpload.InputStream);

                        if (fileResult.IsSuccess)
                            model.FilePath = model.FileToUpload != null ? $@"C:\jobtrackaunz\manuscript\{model.ID}\{model.FileToUpload.FileName}" : string.Empty;

                        var updatedData = await _queryManuscriptService.UpdateQueryManuscriptAsync(model);
                    } 
                }
                else
                {
                    // save file to directory
                    if (model.FileToUpload != null)
                    {
                        fileResult = await WriteToFile($@"C:\jobtrackaunz\manuscript\{model.ID}\{model.FileToUpload.FileName}"
                            , $@"C:\jobtrackaunz\manuscript\{model.ID}\"
                            , model.FileToUpload.InputStream);

                        if (fileResult.IsSuccess)
                            model.FilePath = model.FileToUpload != null ? $@"C:\jobtrackaunz\manuscript\{model.ID}\{model.FileToUpload.FileName}" : string.Empty;
                    }

                    var updatedData = await _queryManuscriptService.UpdateQueryManuscriptAsync(model);
                }                

                // save data to "queryreplies" table with "manuscriptquery" ID as foreign_key
                jsonResult.IsSuccess = await _queryManuscriptService.UpdateQueryReplyAsync(new ReplyModel
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

        public async Task<ActionResult> DeleteQueryManuscript(QueryManuscriptModel model)
        {
            var isSuccess = await _queryManuscriptService.DeleteQueryManuscriptAsync(model);

            return Json(new { success = isSuccess });
        }

        public async Task<ActionResult> UpdateReply(ReplyModel model)
        {
            model.PostedBy = "client";
            var isSuccess = await _queryManuscriptService.UpdateQueryReplyAsync(model);

            if (isSuccess)
                isSuccess = await _queryManuscriptService.UpdateQueryManuscriptStatusAsync(model);

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