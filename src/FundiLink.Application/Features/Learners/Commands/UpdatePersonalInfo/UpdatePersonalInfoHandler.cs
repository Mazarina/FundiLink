using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Learners.Commands.UpdatePersonalInfo;

public class UpdatePersonalInfoHandler : IRequestHandler<UpdatePersonalInfoCommand>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IApplicationDbContext _dbContext;

    public UpdatePersonalInfoHandler(ILearnerRepository learnerRepository, IApplicationDbContext dbContext)
    {
        _learnerRepository = learnerRepository;
        _dbContext = dbContext;
    }

    public async Task Handle(UpdatePersonalInfoCommand request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        learner.UpdatePersonalInfo(
            request.FirstName, request.Surname, request.IdNumber, request.PassportNumber,
            request.Gender, request.HomeLanguage, request.Nationality, request.MobileNumber,
            request.Province, request.Municipality, request.Suburb, request.SchoolName,
            request.SchoolProvince, request.GradeLevel, request.GuardianName,
            request.GuardianPhone, request.GuardianEmail);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
