using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PruebaEfectyFrontend.Models
{
    public class PersonaViewModel
    {
        public int Id { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombres { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellidos { get; set; }

        [Display(Name = "Tipo de Documento")]
        public int TipoDocumento { get; set; }
        public string TipoDocumentoDescripcion { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        public DateTime FechaNacimiento { get; set; }

        [Display(Name = "Valor a ganar")]
        [Required(ErrorMessage = "El valor a ganar es obligatorio")]
        public decimal? ValoraGanar { get; set; }

        public bool EstadoCivil { get; set; }
    }
}
