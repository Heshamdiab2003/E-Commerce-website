﻿using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace MegaMart.Models.ViewModels;
public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
