using ahis.template.application.Shared;
using ahis.template.application.Shared.Mediator;
using ahis.template.domain.Interfaces.Repositories;
using ahis.template.domain.Models.Entities;
using ahis.template.domain.Models.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ahis.template.application.Features.CountryFeatures.Command
{
    public class AddCountryCommand : IRequest<ApiResponse<object>>
    {
        [Required]
        public string CountryFullname { get; set; }
        [Required]
        public string CountryShortname { get; set; }
        public string? CountryDescription { get; set; } = null;
        [Required]
        public string CountryCode2 { get; set; }
        [Required]
        public string CountryCode3 { get; set; }
    }

    public class AddCountryCommandHandler : IRequestHandler<AddCountryCommand, ApiResponse<object>>
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ILogger<AddCountryCommandHandler> _logger;

        public AddCountryCommandHandler(ICountryRepository countryRepository, ILogger<AddCountryCommandHandler> logger)
        {
            _countryRepository = countryRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<object>> Handle(AddCountryCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Handling AddCountryCommandHandler");

                // Map AddCountryCommand (DTO) into Country entity
                Country countryEntity = new Country
                { 
                    CountryFullname = command.CountryFullname,
                    CountryShortname = command.CountryShortname,
                    CountryDescription = command.CountryDescription,
                    CountryCode2 = command.CountryCode2,
                    CountryCode3 = command.CountryCode3
                };

                await _countryRepository.AddAsync(countryEntity);

                return ApiResponse<object>.SuccessResponse(null, $"Country ({countryEntity.CountryCode3} - {countryEntity.CountryShortname}) has been added successfully");
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error occurred while handling AddCountryCommandHandler");


                // Return error response with details
                return ApiResponse<object>.ErrorResponse(
                    message: "Failed to country",
                    errors: new
                    {
                        ExceptionMessage = ex.Message,
                        InnerException = ex.InnerException?.Message,
                        StackTrace = ex.StackTrace
                    });
            }
            
        }
    }
}
