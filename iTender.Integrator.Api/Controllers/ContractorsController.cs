using iTender.Integrator.Application.DTOs;
using iTender.Integrator.Application.DTOs.Crm;
using iTender.Integrator.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iTender.Integrator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractorsController : ControllerBase
    {
        private readonly IContractorRepository _contractorRepository;
        public ContractorsController(IContractorRepository contractorRepository)
        {
            _contractorRepository = contractorRepository;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(PagedResult<ContractorModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                var result = await _contractorRepository.GetByDynamicsAccountIdAsync(id, ct);

                //if (result == null)
                //    return NotFound($"Contractor with id {id} was not found.");

                //var gradesQuery = new GetContractorGradesQuery(result.Id, null, null);
                //result.Grades = await gradesHandler.Handle(gradesQuery, ct) ?? new List<ContractorGradeModel>();

                //var fsQuery = new GetFinancialStatementsByContractorQuery(result.Id);
                //var statements = await fsHandler.Handle(fsQuery, ct) ?? new List<FinancialStatementModel>();

                //var latestStatement = statements
                //    .OrderByDescending(x => x.Year)
                //    .FirstOrDefault();

                //result.AnnualTurnOver = latestStatement?.TurnoverInclVat ?? 0;
                //result.NetAssetValue = latestStatement?.NetAssetValue ?? 0;

                //var cQuery = new GetContactByIdQuery(result.PrimaryContactId.Value);
                //var contact = await contactHandler.Handle(cQuery, ct);

                //result.ContactPersonName = contact.LastName + " " + contact.FirstName;
                //result.ContactPersonEmailAddress = contact.Email;
                //result.ContactPersonTelephone = contact.Telephone;
                //result.ContactPersonMobileNumber = contact.MobilePhone;

                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(StatusCodes.Status499ClientClosedRequest);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred while processing the request.");
            }
        }


        [HttpGet("GetContractorByCrsNumber/{crsNumber}")]
        [ProducesResponseType(typeof(PagedResult<ContractorModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByCRSNumber(string crsNumber, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                var result = await _contractorRepository.GetByCidbRegistrationNumberAsync(crsNumber, ct);

                //if (result == null)
                //    return NotFound($"Contractor with id {id} was not found.");

                //var gradesQuery = new GetContractorGradesQuery(result.Id, null, null);
                //result.Grades = await gradesHandler.Handle(gradesQuery, ct) ?? new List<ContractorGradeModel>();

                //var fsQuery = new GetFinancialStatementsByContractorQuery(result.Id);
                //var statements = await fsHandler.Handle(fsQuery, ct) ?? new List<FinancialStatementModel>();

                //var latestStatement = statements
                //    .OrderByDescending(x => x.Year)
                //    .FirstOrDefault();

                //result.AnnualTurnOver = latestStatement?.TurnoverInclVat ?? 0;
                //result.NetAssetValue = latestStatement?.NetAssetValue ?? 0;

                //var cQuery = new GetContactByIdQuery(result.PrimaryContactId.Value);
                //var contact = await contactHandler.Handle(cQuery, ct);

                //result.ContactPersonName = contact.LastName + " " + contact.FirstName;
                //result.ContactPersonEmailAddress = contact.Email;
                //result.ContactPersonTelephone = contact.Telephone;
                //result.ContactPersonMobileNumber = contact.MobilePhone;

                return Ok(result);
            }
            catch (OperationCanceledException)
            {
                return StatusCode(StatusCodes.Status499ClientClosedRequest);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred while processing the request.");
            }
        }
    }
}
