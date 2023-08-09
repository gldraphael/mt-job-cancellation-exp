using System;

namespace CommonLib.MessageContracts;

public record CancelJobCommand(Guid JobId);
