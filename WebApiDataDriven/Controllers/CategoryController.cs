using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiDataDriven.Data;
using WebApiDataDriven.Models;

namespace WebApiDataDriven.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        public CategoryController() { }

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Category>>> Get([FromServices] DataContext context)
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return Ok(categories);
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById([FromServices] DataContext context, int id)
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return Ok(category);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Post(
            [FromBody]Category model,
            [FromServices]DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception) 
            { 
                return BadRequest(new {message = "Não foi possivel criar esta categoria."});
            };
            
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<Category>>> Put(
            int id, 
            [FromBody] Category model,
             [FromServices] DataContext context)
        {
            if (id != model.Id)
            {
                return NotFound(new { message = " Categoria não encontrada." });
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel atualizar este registro." });
            }

        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<Category>>> Delete(
            int id,
            [FromServices]DataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
                return NotFound(new { message = "Categoria não encontrada." });

            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new {message = "Categoria removida com sucesso."});
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel remover esta categoria." });
            }
        }
    }
}
