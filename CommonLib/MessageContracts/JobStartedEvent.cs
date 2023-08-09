using System;

namespace CommonLib.MessageContracts;

public record JobStartedEvent(string Name, Guid JobId);
