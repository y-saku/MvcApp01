using Microsoft.AspNetCore.Mvc;
using MvcApp01.Models;

namespace MvcApp01.Controllers
{
    public class MovieController:Controller

    {
       public IActionResult Random(){
         var movie = new Movie(){Name="ăăăȘă«"};
         return View(movie);
       } 
    }
}