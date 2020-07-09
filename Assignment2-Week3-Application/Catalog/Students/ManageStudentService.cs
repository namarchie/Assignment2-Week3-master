﻿using Assignment_Week3_Common.Exceptions;
using Assignment2_Week3_Application.Catalog.Students.Dtos.Manage;
using Assignment2_Week3_Application.Dtos;
using Assignment2_Week3_Data.EF;
using Assignment2_Week3_Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2_Week3_Application.Catalog.Students
{
    public class ManageStudentService : IManageStudentService
    {
        private readonly StudentDbContext _context;
        public ManageStudentService(StudentDbContext context)
        {
            _context = context;
        }
        public async Task<int> Create(StudentCreateRequest request)
        {
            var province = _context.Provinces.Find(request.ProvinceId);
            var commune = _context.Communes.Find(request.CommuneId);
            var district = _context.Districts.Find(request.DistrictId);
            //var district = new District()
            //{
            //    DistrictId = request.DistrictId
            //};
            //var commune = new Commune()
            //{
            //    CommuneId = request.CommuneId
            //};
            var student = new Student()
            {
                Name = request.Name,
                Yob = request.Yob,
                Address = request.Address + ", " + province.ProvinceName + ", " +  district.DistrictName + ", " + commune.CommuneName ,
                Phone = request.Phone,
                CommuneId = request.CommuneId,
                DistrictId = request.DistrictId,
                ProvinceId = request.ProvinceId
            };
            _context.Students.Add(student);
             await _context.SaveChangesAsync();
            return student.Id;
            
        }

        public async Task<int> Delete(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null) throw new StudentException("Can not find the student");
            _context.Students.Remove(student);
            return await _context.SaveChangesAsync();

        }

        public async Task<List<StudentViewModel>> GetAll()
        {
            var student =  _context.Students.Select(x => new StudentViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Yob = x.Yob,
                Phone = x.Phone,
                Address = x.Address,
            });


            return await student.ToListAsync();
        }

        public async Task<PagedResult<StudentViewModel>> GetAllPaging(GetStudentPagingRequest request)
        {
            var student = _context.Students.Select(x => new StudentViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Yob = x.Yob,
                Phone = x.Phone,
                Address = x.Address,
            });
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                student = student.Where(p => p.Name.Contains(request.Keyword));
            }
            int totalRow = await student.CountAsync();
            var data = student.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);
            var pageResult = new PagedResult<StudentViewModel>()
            {
                TotalRecord = totalRow,
                Items = await data.ToListAsync()
            };
            return pageResult;



        }

        public async Task<StudentViewModel> GetById(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);
            var studentViewModel = new StudentViewModel()
            {
                Id = studentId,
                Name = student.Name,
                Yob = student.Yob,
                Address = student.Address,
                Phone = student.Phone,
                
            };
            return studentViewModel;
        }

        public async Task<int> Update(StudentUpdateRequest request)
        {
            var province = _context.Provinces.Find(request.ProvinceId);
            var district = _context.Districts.Find(request.DistrictId);
            var commune = _context.Communes.Find(request.CommuneId);
            var student = await _context.Students.FindAsync(request.Id);
            if (student == null) throw new StudentException("Can not find the student");
            student.Name = request.Name;
            student.Phone = request.Phone;
            student.Yob = request.Yob;
            student.Address = request.Address + ", " + province.ProvinceName + ", " + district.DistrictName + ", " + commune.CommuneName;
            return await _context.SaveChangesAsync();

            

        }

        //Task<List<StudentViewModel>> IManageStudentService.GetAll()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
