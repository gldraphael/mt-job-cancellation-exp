using System;

namespace CommonLib.MessageContracts;

public record JobCanceledEvent(Guid JobId, string Name);
