using Microsoft.AspNetCore.Mvc;
using Pasteleria.Abstracciones.Logica.Auditoria;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.LogicaDeNegocio.Auditoria;
using System;
using System.Collections.Generic;

namespace Pasteleria.Controllers
{
    public class AuditoriaController : BaseController
    {
        private IListarAuditorias _listarAuditorias;

        public AuditoriaController()
        {
            try
            {
                _listarAuditorias = new ListarAuditorias();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // GET: Auditoria/ListadoAuditorias
        public IActionResult ListadoAuditorias(string buscar, string filtro)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

            try
            {
                List<Auditoria> auditorias = new List<Auditoria>();

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    switch (filtro)
                    {
                        case "tabla":
                            auditorias = _listarAuditorias.BuscarPorTabla(buscar);
                            break;
                        case "accion":
                            auditorias = _listarAuditorias.BuscarPorAccion(buscar);
                            break;
                        case "usuario":
                            auditorias = _listarAuditorias.BuscarPorUsuario(buscar);
                            break;
                        default:
                            auditorias = _listarAuditorias.BuscarGeneral(buscar);
                            break;
                    }
                    ViewBag.Buscar = buscar;
                    ViewBag.Filtro = filtro;
                }
                else
                {
                    auditorias = _listarAuditorias.Obtener();
                }

                return View(auditorias);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar auditorías: {ex.Message}";
                return View(new List<Auditoria>());
            }
        }
    }
}