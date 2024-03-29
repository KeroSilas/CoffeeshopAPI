﻿using AutoMapper;
using Business.Service;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Entities.DTOs;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    [Route ("GetUsers")]
    public ActionResult GetUsers()
    {
        var users = _userService.GetUsers();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public ActionResult GetUser(Guid id)
    {
        var user = _userService.GetUser(id);
        if (user == null)
        {
            return NotFound();
        }
        var mappedUser = _mapper.Map<GetUserDto>(user);

        return Ok(mappedUser);
    }
    
    [HttpGet("GetUserByUsername/{username}")]
    public ActionResult<GetUserDto> GetUserByUsername(string username)
    {
        var user = _userService.GetUserByUsername(username);
        if (user == null)
        {
            return NotFound();
        }
        var mappedUser = _mapper.Map<GetUserDto>(user);

        return Ok(mappedUser);
    }

    [HttpPut("{id}")]
    public ActionResult<GetUserDto> UpdateUser(Guid id, [FromBody] UpdateUserDto userDto)
    {
        if (!_userService.UserExists(id))
        {
            return NotFound();
        }

        var user = new User();
        _mapper.Map(userDto, user);
        var updatedUser = _userService.UpdateUser(id, user);
        return Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteUser(Guid id)
    {
        if (!_userService.UserExists(id))
        {
            return NotFound();
        }

        _userService.DeleteUser(id);
        return NoContent();
    }

    [HttpPost]
    public ActionResult<GetUserDto> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        var user = new User();
        _mapper.Map(createUserDto, user);
        var createdUser = _userService.CreateUser(user);
        return CreatedAtAction(nameof(GetUser), new {id = createdUser.Id}, createdUser);
    }
    
    [HttpPost("Login")]
    public ActionResult Login([FromBody] LoginDto loginDto)
    {
        var user = _userService.GetUserByUsername(loginDto.Username);
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(loginDto.Password, user.Salt);
        
        if (user == null || !hashedPassword.Equals(user.Password))
        {
            return NotFound();
        }

        return Ok(user);
    }
}