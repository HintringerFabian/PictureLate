using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Tesseract.WebApi.Controllers
{
    public class AnalyseController : ApiController
    {
        // GET: api/Anaylse
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        public HttpResponseMessage PostUserImage()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string text = string.Empty;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 5; //Size = 2 MB

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {

                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                            var message = string.Format("Please Upload a file upto 5 mb.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {
                            //  where you want to attach your imageurl
                            //if needed write the code to update the table
                            string imageName = Guid.NewGuid().ToString();
                            var filePath = HttpContext.Current.Server.MapPath($"~/images/{imageName}" + extension);
                            //Userimage myfolder name where i want to save my image
                            postedFile.SaveAs(filePath);

                            var image = new Bitmap(filePath);
                            TesseractEngine engine = new TesseractEngine(HttpContext.Current.Server.MapPath("~/tessdata"), "deu", EngineMode.Default);
                            Page page = engine.Process(image, PageSegMode.Auto);

                            text = page.GetText();
                        }
                    }

                    return Request.CreateErrorResponse(HttpStatusCode.Created, text); ;
                }
                var res = string.Format("Please Upload an image.");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception ex)
            {
                var res = string.Format(ex.Message + "|InnerException:" + ex.InnerException?.Message);
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }

    }
}
