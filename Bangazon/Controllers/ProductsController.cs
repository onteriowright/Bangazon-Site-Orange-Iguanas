﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangazon.Data;
using Bangazon.Models;
using Bangazon.Models.ProductViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bangazon.Controllers
{


    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: Products
        public async Task<ActionResult> Index(string filter, string searchString)
        {
            var user = await GetUserAsync();
            var products = await _context.Product
                 .Where(p => p.UserId == user.Id)
                 .Include(p => p.ProductType)
                  .ToListAsync();


            switch (filter)
            {
                case "Sporting Goods":
                    products = await _context.Product
                        .Where(p => p.ProductTypeId == 1)
                        .Include(p => p.ProductType)
                        .ToListAsync();
                    break;
                case "Appliances":
                    products = await _context.Product
                        .Where(p => p.ProductTypeId == 2)
                        .Include(p => p.ProductType)
                        .ToListAsync();
                    break;
                case "Tools":
                    products = await _context.Product
                        .Where(p => p.ProductTypeId == 3)
                        .Include(p => p.ProductType)
                        .ToListAsync();
                    break;
                case "Games":
                    products = await _context.Product
                        .Where(p => p.ProductTypeId == 4)
                        .Include(p => p.ProductType)
                        .ToListAsync();
                    break;
                case "Music":
                    products = await _context.Product
                        .Where(p => p.ProductTypeId == 5)
                        .Include(p => p.ProductType)
                        .ToListAsync();
                    break;
                case "Health":
                    products = await _context.Product
                        .Where(p => p.ProductTypeId == 6)
                        .Include(p => p.ProductType)
                        .ToListAsync();
                    break;
                case "Outdoors":
                    products = await _context.Product
                        .Where(p => p.ProductTypeId == 7)
                        .Include(p => p.ProductType)
                        .ToListAsync();
                    break;
                case "Beauty":
                    products = await _context.Product
                        .Where(p => p.ProductTypeId == 8)
                        .Include(p => p.ProductType)
                        .ToListAsync();
                    break;
                case "Shoes":
                    products = await _context.Product
                        .Where(p => p.ProductTypeId == 9)
                        .Include(p => p.ProductType)
                        .ToListAsync();
                    break;
                case "Automotive":
                    products = await _context.Product
                        .Where(p => p.ProductTypeId == 10)
                        .Include(p => p.ProductType)
                        .ToListAsync();
                    break;
                case "Show All":
                    products = await _context.Product
                        .Include(p => p.ProductType)
                        .ToListAsync();
                    break;
            }


            if (searchString != null)
            {
                products = await _context.Product
                      .Where(p => p.Title.Contains(searchString) && p.Active == true || p.City.Contains(searchString) && p.Active == true)
                      .Include(p => p.ProductType)
                       .ToListAsync();
                return View(products);
            }

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var viewModel = new ProductFormViewModel();
            var product = await _context.Product.Include(p => p.ProductType)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            var user = await GetUserAsync();



            viewModel.DateCreated = product.DateCreated;
            viewModel.Description = product.Description;
            viewModel.Title = product.Title;
            viewModel.Price = product.Price;
            viewModel.Quantity = product.Quantity;
            viewModel.UserId = product.UserId;
            viewModel.City = product.City;
            viewModel.ImagePath = product.ImagePath;
            viewModel.Active = product.Active;
            viewModel.ProductTypeId = product.ProductTypeId;
            viewModel.ProductType = product.ProductType;
            viewModel.ProductId = product.ProductId;

            return View(viewModel);
        }

        // GET: Products/ProductTypes
        public async Task<ActionResult> ProductTypes()
        {
            var model = new ProductTypesViewModel();
            model.Types = await _context
                .ProductType
                .Select(pt => new TypeWithProducts()
                {
                    TypeId = pt.ProductTypeId,
                    TypeName = pt.Label,
                    ProductCount = pt.Products.Count(),
                    Products = pt.Products.OrderByDescending(p => p.DateCreated).Take(3)
                }).ToListAsync();

            return View(model);
        }

        // GET: Products/Favorites
        public async Task<ActionResult> Favorites()
        {
            var user = await GetUserAsync();
            var favorites = await _context.UserProducts
                .Where(up => up.UserId == user.Id && up.IsLiked == true)
                .Include(p => p.Product)
                .ToListAsync();

            return View(favorites);
        }



        // GET: Products/Create
        public async Task<ActionResult> Create()
        {
            var viewModel = new ProductFormViewModel();

            var productTypeOptions = await _context.ProductType.Select(g => new SelectListItem()
            {
                Text = g.Label,
                Value = g.ProductTypeId.ToString()
            }).ToListAsync();

            viewModel.ProductTypeOptions = productTypeOptions;
            return View(viewModel);
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductFormViewModel productViewModel)
        {
            try
            {
                var user = await GetUserAsync();
                var product = new Product()
                {
                    DateCreated = DateTime.Now,
                    Description = productViewModel.Description,
                    Title = productViewModel.Title,
                    Price = productViewModel.Price,
                    Quantity = productViewModel.Quantity,
                    UserId = user.Id,
                    City = productViewModel.City,
                    ImagePath = productViewModel.ImagePath,
                    Active = productViewModel.Active,
                    ProductTypeId = productViewModel.ProductTypeId
                };

                _context.Product.Add(product);
                await _context.SaveChangesAsync();

                var id = product.ProductId;

                return RedirectToAction("Details", new { id = id });
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        //POST: Products/AddToOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddToOrder(int ProductId)
        {
            var user = await GetUserAsync();
            var order = await _context.Order.FirstOrDefaultAsync(o => o.UserId == user.Id && o.PaymentTypeId == null);

            var orderProduct = new OrderProduct()
            {
                OrderId = order.OrderId,
                ProductId = ProductId,
            };

            var prod = await _context.Product.FirstOrDefaultAsync(p => p.ProductId == ProductId);
            prod.Quantity = prod.Quantity - 1;

            _context.OrderProduct.Add(orderProduct);
            _context.Product.Update(prod);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        //POST: Products/UserLikePreference
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserLikePreference(int ProductId, bool liked)
        {
            var user = await GetUserAsync();
            var currentPreference = await _context.UserProducts.FirstOrDefaultAsync(p => p.ProductId == ProductId && p.UserId == user.Id);
            if (currentPreference == null)
            {
                var userProduct = new UserProduct()
                {
                    UserId = user.Id,
                    ProductId = ProductId,
                    IsLiked = liked
                };
                _context.UserProducts.Add(userProduct);

            }else
            {
                currentPreference.IsLiked = liked;
                _context.UserProducts.Update(currentPreference);
            }
       
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = ProductId });
        }

        // POST: Products/RemoveFavorite/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveFavorite(UserProduct product)
        {
            try
            {
                var userProduct = await _context.UserProducts.FirstOrDefaultAsync(up => up.UserProductId == product.UserProductId);

                _context.UserProducts.Remove(userProduct);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Favorites));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var product = await _context.Product.Include(p => p.ProductType).FirstOrDefaultAsync(p => p.ProductId == id);
            var user = await GetUserAsync();

            if (product.UserId != user.Id)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Product product)
        {
            try
            {
                _context.Product.Remove(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        private async Task<ApplicationUser> GetUserAsync() => await _userManager.GetUserAsync(HttpContext.User);
    }
}