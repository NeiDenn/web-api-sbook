﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
// imports
using Newtonsoft.Json;
using System.Text;
using ProyectoSBooks.Web.Models;
using System.Net;

namespace ProyectoSBooks.Web.Controllers
{
    public class ProveedorController : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.mensaje = TempData["mensaje"];
            return View(await listadoProveedores());
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.paises = new SelectList(await listadoPaises(), "Idpais", "NombrePais");
            return View(await Task.Run(() => new Proveedor()));
        }

        public async Task<IActionResult> Edit(int IdProveedor)
        {
            Proveedor proveedorExistente = await ObtenerProveedorPorId(IdProveedor);
            if (proveedorExistente == null)
            {
                return NotFound();
            }
            ViewBag.paises = new SelectList(await listadoPaises(), "Idpais", "NombrePais");
            return View(proveedorExistente);
        }

        async Task<List<Proveedor>> listadoProveedores()
        {
            List<Proveedor> objProveedor = new List<Proveedor>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiProveedor/");
                HttpResponseMessage mensaje = await client.GetAsync("proveedores");
                string cadena = await mensaje.Content.ReadAsStringAsync();

                objProveedor = JsonConvert.DeserializeObject<List<Proveedor>>(cadena).Select(
                    c => new Proveedor
                    {
                        IdProveedor = c.IdProveedor,
                        NombreCia = c.NombreCia,
                        NombreContacto = c.NombreContacto,
                        CargoContacto = c.CargoContacto,
                        Direccion = c.Direccion,
                        NombrePais = c.NombrePais,
                        Telefono = c.Telefono,
                        Fax = c.Fax
                    }).ToList();
            }
            return objProveedor;
        }

        async Task<List<Pais>> listadoPaises()
        {
            List<Pais> objPais = new List<Pais>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiPais/");
                HttpResponseMessage mensaje = await client.GetAsync("paises");
                string cadena = await mensaje.Content.ReadAsStringAsync();

                objPais = JsonConvert.DeserializeObject<List<Pais>>(cadena).Select(
                    s => new Pais
                    {
                        Idpais = s.Idpais,
                        NombrePais = s.NombrePais
                    }).ToList();
            }
            return objPais;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Proveedor reg)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiProveedor/");
                StringContent contenido = new StringContent(JsonConvert.SerializeObject(reg), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage msg = await client.PostAsync("agregar", contenido);
                mensaje = await msg.Content.ReadAsStringAsync();
            }
            TempData["mensaje"] = mensaje;
            return RedirectToAction("Index");
        }

        public async Task<Proveedor> ObtenerProveedorPorId(int IdProveedor)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7194/api/apiProveedor/");
                HttpResponseMessage response = await client.GetAsync($"buscar/{IdProveedor}");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Proveedor proveedor = JsonConvert.DeserializeObject<Proveedor>(json);
                    return proveedor;
                }
                else
                {
                    return null;
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int IdProveedor, Proveedor reg)
        {
            try
            {
                string mensaje = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7194/api/");
                    string jsonData = JsonConvert.SerializeObject(reg);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage msg = await client.PutAsync($"apiProveedor/actualizar?IdProveedor={IdProveedor}", content);
                    if (msg.StatusCode == HttpStatusCode.OK)
                    {
                        mensaje = $"Se actualizó correctamente el proveedor con ID {IdProveedor}";
                    }
                    else
                    {
                        mensaje = $"Error al actualizar el proveedor. Detalles: {msg.ReasonPhrase}";
                    }
                }

                TempData["mensaje"] = mensaje;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = "Error al actualizar el proveedor. Detalles: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Delete(int IdProveedor)
        {
            try
            {
                string mensaje = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7194/api/");

                    HttpResponseMessage msg = await client.DeleteAsync($"apiProveedor/eliminar?IdProveedor={IdProveedor}");
                    if (msg.IsSuccessStatusCode)
                    {
                        mensaje = $"Se eliminó correctamente el proveedor con ID {IdProveedor}";
                    }
                    else
                    {
                        mensaje = $"Error al eliminar el proveedor. Detalles: {msg.ReasonPhrase}";
                    }
                }

                TempData["mensaje"] = mensaje;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = "Error al eliminar el proveedor. Detalles: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
