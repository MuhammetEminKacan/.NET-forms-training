using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace _7_FormsApp.Models
{
    public class Product
    {
        [Display(Name ="Urun Id")] // display komutu sitede gosterilecek ismi belirler
       // [BindNever]  // BindNever ile bu alan formdan gelmez, sadece veritabanından gelir post ettiğimizde model e gelmez
        public int ProductId { get; set; }


        [Required(ErrorMessage = "Urun Adi Bos Bırakılamaz!")] // Required ile bos birakilamaz
        [StringLength(50, ErrorMessage = "Urun Adi 50 karakterden uzun olamaz!")] // StringLength ile max 50 karakterden uzun olamaz dedik
        [Display(Name = "Urun Adı")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Urun Fiyatı Bos Bırakılamaz!")]
        [Range(1,100000)]     // Range ile girilen fiyatın minimum 1 maksimum 100 bin sayıları arasında olmalı şartını girdik
        [Display(Name = "Urun Fiyatı")]
        public decimal? Price { get; set; }

        
        [Display(Name = "Urun Resmi")]
        public string? Image { get; set; }

        [Display(Name = "Urun Aktif Mi?")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "kategori kısmı seçilmeli ")]
        [Display(Name = "kategori")]
        public int? CategoryId { get; set; }

    }
}
