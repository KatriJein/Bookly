using Core.Payloads;

namespace Core.Dto.Preferences;

public record PreferencePayloadDto(Guid BookId, Guid UserId, IPrerefenceActionPayload PrerefenceActionPayload);