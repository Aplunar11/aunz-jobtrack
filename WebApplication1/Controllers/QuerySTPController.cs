using JobTrack.Models;
using JobTrack.Models.Enums;
using JobTrack.Models.QueryManuscript;
using JobTrack.Models.QuerySTP;
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
    public class QuerySTPController : Controller
    {
        private readonly IQuerySTPService _querySTPService;
        private readonly INotificationService _notificationService;

        public QuerySTPController(IQuerySTPService querySTPService, INotificationService notificationService)
        {
            _querySTPService = querySTPService;
            _notificationService = notificationService;
        }

        public async Task<ActionResult> Index(int id, int u)
        {
            if (Session["UserName"] == null)
            {
                TempData["alertMessage"] = "You must log in to continue";
                return RedirectToAction("Login", "Login");
            }

            ViewBag.UserAccess = (UserAccessEnum)u;
            ViewBag.StpID = id;
            var viewModel = await _querySTPService.GetSTPDataByIdAsync(id);
            return View(viewModel);
        }

        public async Task<ActionResult> Reply(int id, int queryid, bool v, int u, bool? fi, int? e, int? ni)
        {
            // v (isView)
            // u (userAccess)
            // fi (isFromIndex)
            // e (employeeID)
            // ni (notificationPostId)

            // relogin for new session
            if (Session["UserName"] == null)
            {
                TempData["alertMessage"] = "You must log in to continue";
                return RedirectToAction("Login", "Login");
            }

            // v (isViewOnly)
            var model = new ReplyModel { QueryID = queryid };
            model.Replies = await _querySTPService.GetSTPRepliesAsync(queryid);

            var querystp= await _querySTPService.GetQuerySTPByIdAsync(queryid);
            model.STPStatusID = querystp.STPStatusID.Value;
            model.STPTopicTitle = querystp.STPTopicTitle;
            model.STPType = querystp.STPType;
            model.StpID = id;
            model.IsReplied = querystp.IsReplied;

            ViewBag.UserAccess = (UserAccessEnum)u;
            ViewBag.UserName = !(Session["UserName"] is null) ? Session["UserName"] : "system";
            ViewBag.IsViewOnly = v;
            ViewBag.IsFromIndex = fi.HasValue ? fi : false;

            if (fi.HasValue)
            {
                var data = await _notificationService.UpdateNotificationUserRecordAsync(new NotificationModel { ID = ni.Value, EmployeeID = e.Value });
            }

            return View(model);
        }

        public async Task<ActionResult> _ReplyBox(ReplyModel model)
        {
            return PartialView(await Task.FromResult(model));
        }

        public async Task<string> GetQuerySTP(int stpId, int filterBy)
        {
            var result = new JsonResultModel();
            var filter = (FilterEnum)filterBy;

            try
            {
                result.Collection = await _querySTPService.GetQuerySTPAsync(stpId, filter);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }

            return JsonConvert.SerializeObject(result);
        }

        public async Task<ActionResult> UpdateQuerySTP(QuerySTPModel model)
        {
            var isNew = model.ID < 1;
            var jsonResult = new JsonResultModel();
            var fileResult = new JsonResultModel { IsSuccess = true };
            var currentUser = Session["UserName"] != null ? Session["UserName"].ToString() : "system";
            var filePath = $@"C:\jobtrackaunz\stp\query";

            try
            {
                model.PostedBy = currentUser;

                if (isNew)
                {
                    // save new query and get new ID
                    model.FilePath = string.Empty;  // fix for "Unhandled type encountered"
                    var newData = await _querySTPService.UpdateQuerySTPAsync(model);
                    model.ID = newData.ID;

                    // save file to directory
                    if (model.FileToUpload != null)
                    {
                        fileResult = await WriteToFile($@"{filePath}\{model.ID}\{model.FileToUpload.FileName}"
                            , $@"{filePath}\{model.ID}\"
                            , model.FileToUpload.InputStream);

                        if (fileResult.IsSuccess)
                            model.FilePath = model.FileToUpload != null
                                ? $@"{filePath}\{model.ID}\{model.FileToUpload.FileName}"
                                : string.Empty;

                        var updatedData = await _querySTPService.UpdateQuerySTPAsync(model);
                    }
                }
                else
                {
                    // save file to directory
                    if (model.FileToUpload != null)
                        fileResult = await WriteToFile($@"{filePath}\{model.ID}\{model.FileToUpload.FileName}"
                            , $@"{filePath}\{model.ID}\"
                            , model.FileToUpload.InputStream);

                    if (fileResult.IsSuccess)
                        model.FilePath = model.FileToUpload != null
                            ? $@"{filePath}\{model.ID}\{model.FileToUpload.FileName}"
                            : string.Empty;

                    var updatedData = await _querySTPService.UpdateQuerySTPAsync(model);
                }

                // save data to "coverreplies" table with "coversheetquery" ID as foreign_key
                jsonResult.IsSuccess = await _querySTPService.UpdateSTPReplyAsync(new ReplyModel
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

        public async Task<ActionResult> UpdateReply(ReplyModel model)
        {
            model.PostedBy = Session["UserName"] != null ? (string)Session["UserName"] : "system";
            var isSuccess = await _querySTPService.UpdateSTPReplyAsync(model);

            if (isSuccess)
                isSuccess = await _querySTPService.UpdateQuerySTPStatusAsync(model, model.PreviousStatusID != model.STPStatusID);

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