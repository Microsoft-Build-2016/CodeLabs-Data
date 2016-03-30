using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using MSCorp.AdventureWorks.Core.Repository;

namespace MSCorp.AdventureWorks.Web.Controllers
{
    public class LoadController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadController"/> class.
        /// </summary>
        public LoadController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult Index()
        {
            return View();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"), Route("Load/Start")]
        public async Task<ActionResult> Start(CancellationToken cancelToken = default(CancellationToken))
        {
            ViewBag.Status = "Load completed";
            await _unitOfWork.ExecuteLoad(cancelToken);
            return View("Index");
        }

        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        [Route("Load/End")]
        public async Task<ActionResult> End()
        {
            ViewBag.Status = "Load cancelled";
            CancellationToken cancelToken = new CancellationToken(true);
            try
            {
                await _unitOfWork.ExecuteLoad(cancelToken);
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e);
            }
            return View("Index");
        }
    }
}