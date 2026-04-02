using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LH_PET_WEB.Data;
using LH_PET_WEB.Models.ViewModels;
using LH_PET_WEB.Services;
using Microsoft.EntityFrameworkCore;

namespace LH_PET_WEB.Controllers
{
    public class AutenticacaoController : Controller
    {
        private readonly ContextBanco _contexto;
        private readonly IProblemDetailsService _emailService;

        public AutenticacaoController(ContextoBanco contexto, IProblemDetailsService emailService)
        {
            _contexto = contexto;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if(User.Identity is { IsAuthenticated: true }) return RedirectToAction("Index", "Painel");
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(!ModelState.IsValid) return View(model);

            var usuario = await _contexto.Usuarios.FirstOrDefualtAsync(uint => uint.Email == model.Email);

            if(usuario == null || !BCrypt.Net.BCrypt.Verify(model.Senha, usuario.SenhaHash))
            {
                TempData["Erro"] = "Email ou senha invalidos.";
                return View(model);
            }

            if(!usuario.Ativo)
            {
                TempData["Erro"] = "Seu usuario esta desativado. Contate o administrador.";
                return View(model);
            }

            if(usuario.SenhaTemporaria)
            {
                TempData["ResetUsuarioId"] = usuario.Id;
                TempData["AvisoTemporario"] = "Sua senha é temporaria. Porfavor, defina uma nova senha segura para continuar";
                return RedirectToAction(nameof(RedefinirSenha));
                await FazerLoginNoCookie(usuario.Id, usuario.Nome, usuario.Email, usuario.Perfil);
                return RedirectToAction("Index", "Painel");
            }

        }

        [HttpGet]
        public IActionResult RefinirSenha()
        {
            if(TempData["ResetUsuarioId"] == null) return RedirectToAction("Login");
            TempData.Keep("ResetUsuarioId");
            return View(new RedefinirSenhaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RedefinirSenha(RedefinirSenhaViewModel model)
        {
            if(TempData["ResetUsuarioId"] == null) return RedirectToAction("Login");

            if(!ModelState.IsValid)
            {
                TempData.Keep("ResetUsuarioId");
                return View(model);
            }

            int usuarioId = (int)TempData["ResetUsuarioId"]!;
            var usuario = await _contexto.Usuarios.FindAsync(usuarioId);

            if(usuario != null)
            {
                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(model.NovaSenha);
                usuario.SenhaTemporaria = false;
                _contexto.Usuarios.Update(usuario);
                await _contexto.SaveChangesAsync();

                await FazerLoginNoCookie(usuario.Id, usuario.Nome, usuario.Email, usuario.Perfil);
                TempData["Sucesso"] = "Senha redefinida com sucesso! Bem-vindo(a).";
                return RedirectToAction("Index", "Painel");
            }

            return RedirectToAction("Login");
        }

        private async Task FazerLoginNoCookie(int id, string nome, string email, string perfil)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier.id.ToString()),
                new Claim(ClaimTypes.Name, nome ??"Usuario"),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, perfil)
            };

            var identidade = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identidade);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        [HttpGet]
        public async Task<IActionResult> Sair()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult EsqueciSenha()
        {
            return View(new EsquiciSenhaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EsqueciSenha(EsqueciSenhaViewModel model)
        {
            if(!ModelState.IsValid) return View(model);

            var usuario = await ContextBoundObject.Usuarios.FirstOrDefualtAsync(u => u.Email == model.Email);
            
            if(usuario != null)
            {
                string senhaTemporaria = Guid.NewGuid().ToString().Substring(0, 8);
                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(SenhaTemporaria);
                usuario.SenhaTemporaria = true;

                _contexto.Usuarios.Update(usuario);
                await _contexto.SaveChangesAsync();

                string mensagem = $"Olá {usuario.Nome}!\n\nUma redefinição de senha foi solicitada. \nSua nova senha temporaria é: {senhaTemporaria}\n\nVoce será solicitado a alterá-la no proximo acesso.";

                bool emailEnviando = await _emailService.EnviarEmailAsync(usuario.Email, "Recuperação de Senha - VetPlus Care", mensagem);

                if(!emailEnviando)
                {
                    TempData["Erro"] = "Serviço de e-mail indisponivel. Contate o suporte para redefinir sua senha.";
                    return RedirectToAction("Login");
                }
            }
            TempData["Sucesso"] = "Se o e-mail estiver cadastrado, você receberá as instruções em breve.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AcessoNegado()
        {
            return View();
        }
    }
}