using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcCompany.Data;
using MvcCompany.Models;
using Newtonsoft.Json;

namespace MvcCompany.Controllers
{
    public class EmployeesController : Controller
    {
        string apiUrl = "https://localhost:7220/api/Employee";
        HttpClient client = new HttpClient();

        private readonly MvcCompanyContext _context;

        public EmployeesController(MvcCompanyContext context)
        {
            _context = context;
        }

        //GET: Employees
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            HttpResponseMessage searchEmployees = null;
            HttpResponseMessage responseEmployees = null;
            List<Employees> listEmployees = null;
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                searchEmployees = client.GetAsync(apiUrl + string.Format("/FindEmployees?searchString={0}", searchString)).Result;
            }

            if (String.IsNullOrEmpty(sortOrder))
            {
                ViewData["NameSortParm"] = "name_desc";
                responseEmployees = client.GetAsync(apiUrl + string.Format("/GetAllEmployees")).Result;
            }
            else
            {
                ViewData["NameSortParm"] = "";
                responseEmployees = client.GetAsync(apiUrl + string.Format("/SortEmployees?sortOrder={0}", sortOrder)).Result;
            }
            if (responseEmployees.IsSuccessStatusCode)
            {
                if (searchEmployees != null)
                {
                    listEmployees = JsonConvert.DeserializeObject<List<Employees>>(searchEmployees.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    listEmployees = JsonConvert.DeserializeObject<List<Employees>>(responseEmployees.Content.ReadAsStringAsync().Result);
                }

            }
            int pageSize = 3;
            return View(await PaginatedList<Employees>.CreateAsync(listEmployees, pageNumber ?? 1, pageSize));
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Employees employee = null;

            HttpResponseMessage response = client.GetAsync(apiUrl + string.Format("/GetEmployeesById/id?id={0}", id)).Result;
            if (response.IsSuccessStatusCode)
            {
                employee = JsonConvert.DeserializeObject<Employees>(response.Content.ReadAsStringAsync().Result);
            }
            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,EmployeeFirstName,EmployeeLastName,Salary,Designation,Age")] Employees employee)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync<Employees>(apiUrl + string.Format(@"/CreateEmployees"), employee);
                    //HTTP POST
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Employees employee = null;

            HttpResponseMessage response = client.GetAsync(apiUrl + string.Format("/GetEmployeesById/id?id={0}", id)).Result;
            if (response.IsSuccessStatusCode)
            {
                employee = JsonConvert.DeserializeObject<Employees>(response.Content.ReadAsStringAsync().Result);
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,EmployeeFirstName,EmployeeLastName,Salary,Designation,Age")] Employees employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync<Employees>(apiUrl + string.Format(@"/UpdateEmployees"), employee);
                    //HTTP POST
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Employees employee = null;

            HttpResponseMessage response = client.GetAsync(apiUrl + string.Format("/Delete/id?id={0}", id)).Result;
            if (response.IsSuccessStatusCode)
            {
                employee = JsonConvert.DeserializeObject<Employees>(response.Content.ReadAsStringAsync().Result);
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpResponseMessage response = client.DeleteAsync(apiUrl + string.Format("/DeleteEmployee/id?id={0}", id)).Result;
            //HTTP POST
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return Problem("Entity set 'MvcCompanyContext.Employees'  is null.");
        }

        private bool EmployeesExists(int id)
        {
            return (_context.Employees?.Any(e => e.EmployeeId == id)).GetValueOrDefault();
        }
        private static void Dump(object o)
        {
            string json = JsonConvert.SerializeObject(o, Formatting.Indented);
            Console.WriteLine(json);
        }
    }
}
