﻿using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;

namespace Ntickets.Application.Services.Internal.Base.Inputs;

public interface IServiceInput
{
    public MethodResult<INotification> GetInputValidation();
}
