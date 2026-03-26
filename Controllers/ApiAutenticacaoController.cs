using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using LH_PET_WEB.Data;
using LH_PET_WEB.Models;
using LH_PET_WEB.Models.ViewModels;

namespace LH_PET_WEB.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class ApiAutenticacaoController : ControllerBase
    {
        private readonly ContextoBanco _contexto;
        private readonly IConfiguration _configuracao;

        public ApiAutenticacaoController(ContextoBanco contexto, IConfiguration configuracao)
        {
            _contexto = contexto;
            _configuracao = configuracao;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] ApiRegistroDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var usuarioExiste = await _contexto.Usuarios.AnyAsync(u => u.Email == dto.Email);

                if (usuarioExiste) return BadRequest(new { mensagem = "Email já esta em uso."});

                var cpfExiste = await _contexto.Clientes.AnyAsync(c => c.Cpf == dto.Cpf);
                
                if (cpfExiste) return BadRequest(new { mensagem = "CPF já cadastrado." });

                var novoUsuario = new Usuario
                {
                    Nome = dto.Nome,
                    Email = dto.Email,
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha), 
                    Perfil = "Cliente",
                    Ativo = true,
                    SenhaTemporaria = false
                };

                _contexto.Usuarios.Add(novoUsuario);

                await _contexto.SaveChangesAsync();

                var novoCliente = new ClientErrorData
                {
                    UsuarioId = novoUsuario.Id,
                    Nome = dto.Nome,
                    Cpf = dto.Cpf,
                    Telefone = dto.Telefone
                };

                _contexto.Clientes.Add(novoCliente);
                await _contexto.SaveChangesAsync();

                return Ok(new { mensagem = "Conta criada com sucesso!" });
            } catch (Exception ex)
            {
                string erroReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { mensagem = "Erro interno ao tentar salvar no banco de dados.", detalhe = erroReal });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login ([FromBody] ApiLoginDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest (ModelState);

            try
            {
                var usuario = await _contexto.Usuarios.FirstOrDefualtAsync(u => u.Email == dto.Email);

                if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
                {
                    return Unauthorized (new { mensagem = "Crendenciais invalidas." });
                }

                if (!usuario.Ativo)
                {
                    return Unauthorized(new { mensagem = "Sua conta esta bloqueada."});
                }
            }
        }
    }
}