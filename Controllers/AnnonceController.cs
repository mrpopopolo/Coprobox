﻿using coproBox.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace coproBox.Controllers
{
    public class AnnonceController : Controller
    {
        private IDal dal;
        private IWebHostEnvironment _webEnv;
        public AnnonceController(IWebHostEnvironment environment)
        {
            _webEnv = environment;
            this.dal = new Dal();
        }

        public IActionResult Index()

        {
            List<Annonce> listeDesAnnonces = dal.ObtientToutesLesAnnonces();
            return View(listeDesAnnonces);
        }

        public IActionResult SortIndex()

        {
            List<Annonce> listeDesAnnonces = dal.ObtientToutesLesAnnonces();
            listeDesAnnonces = listeDesAnnonces.OrderByDescending(a => a.TypeService).ToList();
            return View("Index", listeDesAnnonces);
        }

        public IActionResult TriIndex(string searchString)

        {
            
            List<Annonce> listeDesAnnonces = dal.ObtientToutesLesAnnonces();
            if (!String.IsNullOrEmpty(searchString))
            {
                listeDesAnnonces = listeDesAnnonces.Where(a => a.TypeService.ToString().ToLower().Contains(searchString.ToLower())).ToList();
            }
            return View("Index", listeDesAnnonces);
        }


        public IActionResult CreerAnnonce()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreerAnnonce(Annonce annonce)
        {
            
            if (!ModelState.IsValid)
                return View(annonce);

           /* A TESTER LUNDI 05/09/2022
            * string uploads = Path.Combine(_webEnv.WebRootPath, "Image");
            string filePath = Path.Combine(uploads, annonce.Image.FileName);
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                annonce.Image.CopyTo(fileStream);
            }*/
            dal.CreerAnnonce(annonce.Titre, annonce.Description, annonce.TauxHoraire, annonce.Tarif, annonce.DateDebut, annonce.DateFin, annonce.TypeService);

            return RedirectToAction("Index");

        }

        //modifier
        public ActionResult Modifier(int? id)
        {
            if (id.HasValue)
            {
                Annonce annonce = dal.ObtientToutesLesAnnonces().FirstOrDefault(a => a.Id == id.Value);
                if (annonce == null)
                    return View("Error");

                return View(annonce);
            }
            else
                return NotFound();
        }

        [HttpPost]
        public ActionResult Modifier(Annonce annonce)
        {
            if (!ModelState.IsValid)
                return View(annonce);

            if (annonce.Image != null)
            {
                if (annonce.Image.Length != 0)
                {
                    string uploads = Path.Combine(_webEnv.WebRootPath, "Image");
                    string filePath = Path.Combine(uploads, annonce.Image.FileName);
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        annonce.Image.CopyTo(fileStream);
                    }
                    dal.ModifierAnnonce(annonce.Id, annonce.Titre,annonce.Description, annonce.TauxHoraire,annonce.Tarif, annonce.DateDebut, annonce.DateFin,annonce.TypeService, "/Image/" + annonce.Image.FileName);
                  //  string titre, string description, string tauxHoraire, int tarif, DateTime dateDebut, DateTime dateFin, TypeService typeService, int id = 0
                }
            }
            else
            {
                dal.ModifierAnnonce(annonce.Id, annonce.Titre, annonce.Description, annonce.TauxHoraire, annonce.Tarif, annonce.DateDebut, annonce.DateFin, annonce.TypeService, annonce.ImagePath);
            }
            return RedirectToAction("Index");
        }

        /* Définir qui peut supprimer 
        [Authorize(Roles = "Admin")]
        public ActionResult Supprimer(int id)
        {
            dal.SupprimerAnnonce(id);
            return RedirectToAction("Index");
        }*/
        public ActionResult Supprimer(int id)
        {
            dal.SupprimerAnnonce(id);
            return RedirectToAction("Index");
        }
    }
}
