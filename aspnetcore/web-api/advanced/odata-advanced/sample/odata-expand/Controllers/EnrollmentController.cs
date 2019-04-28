﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Models;
using ContosoUniversity.ODataValidators;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly SchoolContext context;
        public EnrollmentController(SchoolContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [MyEnableQuery]
        public IQueryable<Enrollment> Get() => context.Enrollment;
    }
}