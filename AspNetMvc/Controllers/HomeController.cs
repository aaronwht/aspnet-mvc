﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Demo.Models;

namespace Demo.Controllers
{
    // The code included in this project is oversimplified for educational purposes only.
    // Controller Actions should be lean - as thin as possible; you shouldn't be accessing your
    // database and manipulating data.
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var persons = new List<Person>();

            using (IDbConnection conn = new SqlConnection(Config.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    // NOT PRODUCTION WORTHY - DON'T WRITE IN LINE SQL!!!
                    // Demonstrates a simplification for educational purposes
                    cmd.CommandText = "SELECT PersonId, FirstName, LastName FROM Person";
                    cmd.CommandType = CommandType.Text;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // If an error exists using projection you can't unwind it
                            // to learn the line number - YIKES!
                            // Additionally - this is very limited - YUCK!
                            persons.Add(new Person()
                            {
                                PersonId = Convert.ToInt32(reader["PersonId"]),
                                FirstName = Convert.ToString(reader["FirstName"]),
                                LastName = Convert.ToString(reader["LastName"])
                            });
                        }
                    }
                }
            }

            return View(persons);
        }

        public ActionResult Switch()
        {
            var persons = new List<Person>();

            using (IDbConnection conn = new SqlConnection(Config.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    // NOT PRODUCTION WORTHY - DON'T WRITE IN LINE SQL!!!
                    // Demonstrates a simplification for educational purposes
                    cmd.CommandText = "SELECT PersonId, FirstName, LastName FROM Person ORDER BY FirstName";
                    cmd.CommandType = CommandType.Text;
                    using (var reader = cmd.ExecuteReader())
                    {
                        Person person;
                        while (reader.Read())
                        {
                            person = new Person();

                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                // We're not using projection, but this is still pretty terrible!
                                switch (reader.GetName(i))
                                {
                                    case "PersonId":
                                        person.PersonId = Convert.ToInt32(reader["PersonId"]);
                                        break;
                                    case "FirstName":
                                        person.FirstName = Convert.ToString(reader["FirstName"]);
                                        break;
                                    case "LastName":
                                        person.LastName = Convert.ToString(reader["LastName"]);
                                        break;
                                    default:
                                        break;
                                }
                            }

                            persons.Add(person);
                        }
                    }

                }
            }

            return View(persons);
        }

        public ActionResult orm()
        {
            var persons = new List<Person>();

            using (IDbConnection conn = new SqlConnection(Config.ConnectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    // NOT PRODUCTION WORTHY - DON'T WRITE IN LINE SQL!!!
                    // Demonstrates a simplification for educational purposes
                    cmd.CommandText = "SELECT TOP 5 PersonId, FirstName, LastName FROM [Person]";
                    cmd.CommandType = CommandType.Text;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            GenericHelper<Person>.BuildGenericList(ref persons, reader);
                    }
                }
            }

            return View(persons);
        }

        public ActionResult ormstoredprocedure()
        {
            var persons = new List<Person>();

            using (IDbConnection conn = new SqlConnection(Config.ConnectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "dbo.Persons_Get";
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            GenericHelper<Person>.BuildGenericList(ref persons, reader);
                    }
                }
            }

            return View(persons);
        }

        public JsonResult json()
        {
            var persons = new List<Person>();

            using (IDbConnection conn = new SqlConnection(Config.ConnectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    // Using a Stored Procedure
                    cmd.CommandText = "dbo.Persons_Get";
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            GenericHelper<Person>.BuildGenericList(ref persons, reader);
                    }
                }
            }

            return Json(persons, JsonRequestBehavior.AllowGet);
        }

        public JsonResult leanjson()
        {
            var persons = new List<Person>();

            using (IDbConnection conn = new SqlConnection(Config.ConnectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    // Using a Stored Procedure
                    cmd.CommandText = "dbo.Persons_Get";
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            GenericHelper<Person>.BuildGenericList(ref persons, reader);
                    }
                }
            }

            // Projects only the properties we want to return
            var leanPersons = persons.ConvertAll(m => new
            {
                FirstName = m.FirstName,
                LastName = m.LastName
            });

            return Json(leanPersons, JsonRequestBehavior.AllowGet);
        }

        public JsonResult personsphones()
        {
            var persons = new List<Person>();

            using (IDbConnection conn = new SqlConnection(Config.ConnectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    // Stored Prod using an outer apply
                    cmd.CommandText = "dbo.Persons_PhonesCsv";
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            GenericHelper<Person>.BuildGenericList(ref persons, reader);
                    }
                }
            }

            return Json(persons, JsonRequestBehavior.AllowGet);
        }

        public JsonResult personbyemail()
        {
            var person = new Person();

            using (IDbConnection conn = new SqlConnection(Config.ConnectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    // Stored Prod using an outer apply
                    cmd.CommandText = "dbo.Person_ByEmail";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Email", "steveharvey@crazyfunny.com"));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            GenericHelper<Person>.BuildGenericObject(ref person, reader);
                    }
                }
            }

            var leanPerson = new
            {
                FirstName = person.FirstName,
                LastName = person.LastName
            };

            return Json(leanPerson, JsonRequestBehavior.AllowGet);
        }
    }
}