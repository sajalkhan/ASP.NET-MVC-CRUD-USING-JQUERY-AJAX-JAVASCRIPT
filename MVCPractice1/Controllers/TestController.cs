using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCPractice1.Models;
using PagedList;

namespace MVCPractice1.Controllers
{
    public class TestController : Controller
    {
        // GET: Test

        public ActionResult Start()
        {
            Session["view"] = "Start";
            return View();
        }

        public ActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            ViewBag.count = pageNumber;

            using (EmployeesEntities1 db = new EmployeesEntities1())
            {
                List<Employee> employeeList = db.Employees.ToList();
                List<EmployeeViewModel> employeeVmList = db.Employees.Where(x => x.isDeleted == 0)
                    .Select(x => new EmployeeViewModel
                    {
                        Name = x.Name,
                        Address = x.Address,
                        EmployeeId = x.EmployeeId,
                        DepartmentName = x.Department.DepartmentName
                    }).ToList();


                //cms video 31 set pagination ek page theke arek page a jowar jonno (*note instal pagedlist.mvc form naugat packeg)
                var employeePageList = employeeVmList.ToPagedList(pageNumber, 4);
                ViewBag.OnePageInfo = employeePageList;

                return PartialView("Index", employeeVmList);
            }
        }

        public ActionResult AddNewUser(EmployeeViewModel model)
        {
            EmployeesEntities1 db = new EmployeesEntities1();
            List<Department> list = db.Departments.ToList();
            List<Country> countryList = db.Countries.ToList();


            ViewBag.departmentList = new SelectList(list, "DepartmentId", "DepartmentName");
            ViewBag.countryList = new SelectList(countryList, "CountryId", "CountryName");

            return PartialView("AddNewUser", model);
        }

        public ActionResult GetCityList(int countryId)
        {
            EmployeesEntities1 db = new EmployeesEntities1();
            List<City> citylList = db.Cities.Where(x => x.CountryId == countryId).ToList();

            ViewBag.citytList = new SelectList(citylList, "CityId", "CityName");

            return PartialView("GetCityList");
        }

        [HttpPost]
        public ActionResult SaveUserInfo(EmployeeViewModel model)
        {
            var result = "Successfully Added!";

            using (EmployeesEntities1 db = new EmployeesEntities1())
            {
                Employee employee = new Employee();

                employee.Name = model.Name;
                employee.Address = model.Address;
                employee.DepartmentId = model.DepartmentId;
                employee.isDeleted = 0;

                Country country = db.Countries.SingleOrDefault(x => x.CountryId == model.CountryId);
                if (country != null) employee.Country = country.CountryName;

                City city = db.Cities.SingleOrDefault(x => x.CityId == model.CityId);
                if (city != null) employee.City = city.CityName;

                db.Employees.Add(employee);


                #region UploadImg

                var file = model.ImageUpload;

                if (file != null)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    //var extension = Path.GetExtension(file.FileName);
                    //var fileWithourExtension = Path.GetFileNameWithoutExtension(file.FileName);

                    employee.imagePath = fileName;
                    file.SaveAs(Server.MapPath("/images/Uploaded_Image/" + file.FileName));

                    db.Employees.Add(employee);
                }

                #endregion

                db.SaveChanges();
                int id = employee.EmployeeId;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetSearchRecord(string searchText)
        {

            EmployeesEntities1 db = new EmployeesEntities1();


            List<EmployeeViewModel> employeeViewModel = db.Employees.Where(
                x => x.Name.ToLower().Contains(searchText.ToLower()) ||
                     x.Address.ToLower().Contains(searchText.ToLower()) ||
                     x.Department.DepartmentName.ToLower().Contains(searchText.ToLower())).Select(
                x => new EmployeeViewModel
                {
                    Name = x.Name,
                    EmployeeId = x.EmployeeId,
                    DepartmentName = x.Department.DepartmentName,
                    Address = x.Address
                }).ToList();

            return PartialView("GetSearchRecord", employeeViewModel);
        }

        [HttpPost]
        public JsonResult EditUser(EmployeeViewModel model)
        {
            string result = "fail";

            if (!ModelState.IsValid)
            {
                EmployeesEntities1 db = new EmployeesEntities1();

                List<Department> list = db.Departments.ToList();
                List<Country> countryList = db.Countries.ToList();

                ViewBag.departmentList = new SelectList(list, "DepartmentId", "DepartmentName");
                ViewBag.countryList = new SelectList(countryList, "CountryId", "CountryName");

                return Json(result, JsonRequestBehavior.AllowGet);
            }

            using (EmployeesEntities1 db = new EmployeesEntities1())
            {
                if (model.EmployeeId > 0)
                {
                    Employee emp =
                        db.Employees.SingleOrDefault(x => x.isDeleted == 0 && x.EmployeeId == model.EmployeeId);
                    emp.Name = model.Name;
                    emp.Address = model.Address;
                    emp.DepartmentId = model.DepartmentId;

                    Country country = db.Countries.SingleOrDefault(x => x.CountryId == model.CountryId);
                    if (country != null) emp.Country = country.CountryName;

                    City city = db.Cities.SingleOrDefault(x => x.CityId == model.CityId);
                    if (city != null) emp.City = city.CityName;


                    result = "pass";

                    db.SaveChanges();
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditPartialUser(int employeeId)
        {
            EmployeeViewModel model = new EmployeeViewModel();

            using (EmployeesEntities1 db = new EmployeesEntities1())
            {
                List<Department> list = db.Departments.ToList();
                ViewBag.departmentList = new SelectList(list, "DepartmentId", "DepartmentName");

                List<Country> countryList = db.Countries.ToList();
                ViewBag.countryList = new SelectList(countryList, "CountryId", "CountryName");


                if (employeeId > 0)
                {
                    Employee employee = db.Employees.SingleOrDefault
                    (
                        x => x.EmployeeId == employeeId && x.isDeleted == 0
                    );

                    //city id r country id ber korar jonno kaj korci
                    City city = db.Cities.SingleOrDefault(x => x.CityName == employee.City);
                    Country country = db.Countries.SingleOrDefault(x => x.CountryName == employee.Country);

                    if (city != null)
                    {
                        List<City> citylList = db.Cities.Where(x => x.CountryId == country.CountryId).ToList();
                        ViewBag.cityList = new SelectList(citylList, "CityId", "CityName");
                    }
                    else
                    {
                        //pass empty list but not null
                        List<City> citylList = new List<City>();
                        ViewBag.cityList = new SelectList(citylList, "CityId", "CityName");
                    }

                    if (employee != null)
                    {
                        model.EmployeeId = employee.EmployeeId;
                        model.DepartmentId = employee.DepartmentId;
                        model.Name = employee.Name;
                        model.Address = employee.Address;
                        if (country != null) model.CountryId = country.CountryId;
                        if (city != null) model.CityId = city.CityId;
                    }
                }
            }

            return PartialView("EditPartialUser", model);
        }

        public ActionResult ShowPartialUser(int employeeId)
        {
            EmployeesEntities1 db = new EmployeesEntities1();
            List<EmployeeViewModel> empVm = db.Employees.Where(x => x.isDeleted == 0 && x.EmployeeId == employeeId)
                .Select(x => new EmployeeViewModel
                {
                    EmployeeId = x.EmployeeId,
                    Name = x.Name,
                    Address = x.Address,
                    CountryName = x.Country,
                    CityName = x.City,
                    DepartmentName = x.Department.DepartmentName,
                    imagePath = x.imagePath
                }).ToList();

            return PartialView(empVm);
        }

        [HttpPost]
        public JsonResult DeleteUser(int EmployeeId)
        {
            bool result = false;
            EmployeesEntities1 db = new EmployeesEntities1();
            Employee employee = db.Employees.SingleOrDefault(x => x.isDeleted == 0 && x.EmployeeId == EmployeeId);

            if (employee != null)
            {
                employee.isDeleted = 1;
                db.SaveChanges();
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (EmployeesEntities1 db = new EmployeesEntities1())
            {
                UserTbl user = new UserTbl();
                user.UserName = model.UserName;
                user.Address = model.Address;
                user.Email = model.Email;
                user.Password = model.Password;
                user.RoleId = 3;

                if (user.Password != model.ConfirmPassword)
                {
                    return View(model);
                }

                db.UserTbls.Add(user);
                db.SaveChanges();
            }

            return View(model);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(LoginViewModel model)
        {
            string result = "fail";

            if (!ModelState.IsValid)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            using (EmployeesEntities1 db = new EmployeesEntities1())
            {
                UserTbl user = db.UserTbls.SingleOrDefault(x => x.Email == model.Email && x.Password == model.Password);

                if (user != null)
                {
                    Session["userId"] = user.UserId;
                    Session["userName"] = user.UserName;
                    if (user.RoleId == 3)
                    {
                        result = "GeneralUser";
                    }
                    else if (user.RoleId == 1)
                    {
                        result = "Admin";
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login");
        }
    }
}