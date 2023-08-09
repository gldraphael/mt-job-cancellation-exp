using System;

namespace CommonLib.MessageContracts;

public record JobRunningEvent(
    string Name,
    Guid JobId
);
