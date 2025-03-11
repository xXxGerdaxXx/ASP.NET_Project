//using Microsoft.AspNetCore.Mvc;
//using MainApp.Models;

//namespace MainApp.Controllers;

//public class AuthController : Controller
//{
//    private SignUpViewModel _signUpViewModel;

//    public AuthController(SignUpViewModel signUpViewModel)
//    {
//        _signUpViewModel = signUpViewModel;
//    }

//    public IActionResult SignUp()
//    {
//        return View(_signUpViewModel);
//    }

//    [HttpPost]
//    public IActionResult SignUp(SignUpFormModel formData)
//    {
//        if (!ModelState.IsValid)
//        {
//            _signUpViewModel.FormData = formData;
//            return View(_signUpViewModel);
//        }
//        // Save the user to the database
//        return RedirectToAction("Index", "Home");
//    }
//}
