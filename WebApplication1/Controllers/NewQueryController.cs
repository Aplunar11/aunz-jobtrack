﻿using JobTrack.Models;
using JobTrack.Models.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace JobTrack.Controllers
{
    public class NewQueryController : Controller
    {
        QueryModel qm = new QueryModel();
        BaseModel bm = new BaseModel();
        public int mode = 0;
        // GET: Query
        // your data table

        // your method to pull data from database to datatable   
        public void PullData()
        {
            string connString = @"your connection string here";
            string query = "SELECT * FROM tbl_query";

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
        }
        // create data adapter
        public ActionResult QueryManuscript()
        {
            //var row = qm.GetQueryList((int)Session["jobId"]);
            return View();
                
        }


        public ActionResult QueryIndex(int id = 1)
        {
            Session["status"] = "new";
            this.ViewBag.Job = id;
            this.ViewBag.TopicTitle = new SelectList(bm.GetTopic(), "topic_id", "topic");

            Session["jobId"] = id;

            var row = qm.GetQueryList(id);
            return View(row);
        }
        [HttpGet]
        public ActionResult QueryCreate(int id)
        {
            Session["status"] = "new";
            this.ViewBag.Job = id;
            this.ViewBag.TopicTitle = new SelectList(bm.GetTopic(), "topic_id", "topic");
            return View();
        }
        [HttpPost]
        public ActionResult QueryCreate(FormCollection fc, HttpPostedFileBase file)
        {
            string _path = "";
            bool IsValid = false;
            this.ViewBag.Job = fc["task"];
            this.ViewBag.TopicTitle = new SelectList(bm.GetTopic(), "topic_id", "topic");

            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(file.FileName);
                    _path = Path.Combine(Server.MapPath("~/Files"), _FileName);
                    file.SaveAs(_path);

                    IsValid = true;
                }
            }
            else { IsValid = true; }


            if (IsValid)
            {
                if (fc["query"].Length > 0)
                {
                    QueryCreateModel qcm = new QueryCreateModel();

                    int topic_id = 0;
                    if (fc["topic"] != "") { topic_id = Convert.ToInt32(fc["topic"]); } else { topic_id = 0; }
                    if (fc["task"] != "") { qcm.Task = fc["task"]; } else { qcm.Task = " "; }

                    qcm.Query = fc["query"];

                    if (fc["topic"] != "")
                    {
                        var row_topic = bm.GetTopic().Where(model => model.topic_id == topic_id).FirstOrDefault();
                        qcm.Topic = row_topic.topic;
                    }
                    else { qcm.Topic = " "; }

                    qcm.file = _path;
                    qcm.PostedBy = (int)Session["id"];

                    if (((string)Session["status"]) == "new")
                    {
                        Session["qid"] = qm.AddQuery(qcm);

                        Session["status"] = "update";

                        return View();

                    }
                    else
                    {
                        int query_id = qm.updateQuery(qcm, ((int)Session["qid"]));
                        return View();
                    }

                }
            }

            return View();


        }
        public ActionResult QueryList()
        {
            var row = qm.GetQueryList((int)Session["jobId"]);
            return View(row);
        }
        public ActionResult QueryView(int queryid, int jobId)
        {
            this.ViewBag.Task = queryid;
            this.ViewBag.Job = jobId;
            var row = qm.GetSubQueryList(queryid);
            this.ViewBag.Topic = row[0].Topic;
            return View(row);
        }
    }
}