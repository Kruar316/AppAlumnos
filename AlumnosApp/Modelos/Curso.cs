using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlumnosApp.Modelos
{
    public class Curso
    {
        public string? Nombre { get; set; }
        public bool? Estado { get; set; }
    }
}