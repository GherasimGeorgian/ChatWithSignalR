﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatWithSignalR.Hubs;
using ChatWithSignalR.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatWithSignalR.Controllers
{
    public class AccountController : Controller
    {
        SignInManager<User> _signInManager;
        UserManager<User> _userManager;
        IHubContext<ChatHub> _chat;
        public AccountController(IHubContext<ChatHub> chat, UserManager<User> userManager,SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _chat = chat;
        }

            [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null) {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

                if (result.Succeeded)
                {
                   
                    return RedirectToAction("Index", "Home");
                }
               
            }
            return RedirectToAction("Login", "Account");
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new User
            {
                UserName = username
            };
            var result = await _userManager.CreateAsync(user,password);
            if (result.Succeeded){
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Register","Account");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); 
            return RedirectToAction("Login", "Account");
        }
    }
}
