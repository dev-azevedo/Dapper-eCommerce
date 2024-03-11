using eCommerce.API.Model;
using eCommerce.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.Controllers
{
    [Route("api/Contrib/[controller]")]
    [ApiController]
    public class ContribUsuariosController : ControllerBase
    {
        private IUsuarioRepository _usuarioRepository;

        public ContribUsuariosController()
        {
            _usuarioRepository = new ContribUsuarioRepository();
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_usuarioRepository.Get());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Usuario usuario = _usuarioRepository.Get(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Usuario usuario)
        {
            _usuarioRepository.Insert(usuario);
            return Ok(usuario);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Usuario usuario)
        {
            _usuarioRepository.Update(usuario);
            return Ok(usuario);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _usuarioRepository.Delete(id);
            return Ok();
        }
    }
}
