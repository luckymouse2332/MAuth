using Mapster;
using MAuth.Contracts.Enums;
using MAuth.Web.Commons.Helpers;
using MAuth.Web.Controllers.Players.DTOs;
using MAuth.Web.Controllers.Players.Requests;
using MAuth.Web.Data.Entities;

namespace MAuth.Web.Controllers.Players;

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
