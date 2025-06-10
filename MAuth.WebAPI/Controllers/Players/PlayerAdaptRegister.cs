using Mapster;
using MAuth.Data.Entities;
using MAuth.WebAPI.Commons.Helpers;
using MAuth.WebAPI.Controllers.Players.DTOs;
using MAuth.WebAPI.Controllers.Players.Requests;

namespace MAuth.WebAPI.Controllers.Players;

public class PlayerAdaptRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<Player, PlayerDto>()
            .Map(dest => dest.Status, src => src.Status.ToString());

        config.ForType<PlayerUpdateRequest, Player>()
            .Map(dest => dest.Status, src => EnumHelper.ParseEnum<PlayerStatus>(src.Status));

        config.ForType<PlayerCreateRequest, Player>();
    }
}
