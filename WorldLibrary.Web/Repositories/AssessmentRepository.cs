using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorldLibrary.Web.Data;
using WorldLibrary.Web.Data.Entities;
using WorldLibrary.Web.Models;

namespace WorldLibrary.Web.Repositories
{
    public class AssessmentRepository : GenericRepository<Assessment>, IAssessmentRepository
    {
        private readonly DataContext _context;
        private readonly IReserveRepository _reserveRepository;
        public AssessmentRepository(DataContext context,
            IReserveRepository reserveRepository) : base(context)
        {
            _context= context;
            _reserveRepository= reserveRepository;
        }

        /// <summary>
        /// Método que preenche o combobox com as opções de avaliação
        /// </summary>
        /// <returns>Classificação</returns>
        public IEnumerable<SelectListItem> GetComboAssessment()
        {

            var model = new AssessmentViewModel
            {
                Assessments = new List<SelectListItem>
                {
                    new SelectListItem{Text = "Select the Classification...",Value = "" },
                    new SelectListItem{Text = "5 *", Value = "5 *"},
                    new SelectListItem{Text = "4 *", Value = "4 *"},
                    new SelectListItem{Text = "3 *", Value = "3 *"},
                    new SelectListItem{Text = "2 *", Value = "2 *"},
                    new SelectListItem{Text = "1 *", Value = "1 *"},


                },

            };
            return model.Assessments;

        }


        /// <summary>
        /// Método que adiciona a avaliação feita pelo clientes aos livros 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task AddAssessmentAsync(AssessmentViewModel model)
        {
            var reserve = _reserveRepository.GetReserveByIdAsync(model.Reserve.Id);

            var book = await _context.Books.FindAsync(reserve.Result.Book.Id);
            if (book == null)
            {
                return;
            }

            var assessment = new AssessmentViewModel
            {
                IdAssessment = model.IdAssessment,
                AssessmentId = model.AssessmentId,
                Reserve = model.Reserve,
            };

            book.Assessment = assessment.AssessmentId;
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        
    }
}
