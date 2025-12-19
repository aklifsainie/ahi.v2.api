using ahis.template.application.Shared;
using ahis.template.application.Shared.Mediator;
using ahis.template.application.Interfaces.Repositories;
using ahis.template.domain.Models.Entities;
using ahis.template.domain.Models.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentResults;

namespace ahis.template.application.Features.CountryFeatures.Command
{
    public class AddCountryCommand : IRequest<Result<AddCountryCommand>>
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

    public class AddCountryCommandHandler : IRequestHandler<AddCountryCommand, Result<AddCountryCommand>>
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ILogger<AddCountryCommandHandler> _logger;

        public AddCountryCommandHandler(ICountryRepository countryRepository, ILogger<AddCountryCommandHandler> logger)
        {
            _countryRepository = countryRepository;
            _logger = logger;
        }

        public async Task<Result<AddCountryCommand>> Handle(AddCountryCommand command, CancellationToken cancellationToken)
        {


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

            return Result.Ok(command)
                .WithSuccess("Country has been added successfully.");


        }
    }
}
