#include "Common.h"

#include "Common/CommonHeaders.h"
#include "Common/Identifier.h"
#include "GameEntity/Entity.h"
#include "GameEntity/Transform.h"
#include "GameEntity/Script.h"

using namespace fabric;

namespace
{
	struct transform_component
	{
		f32 position[3];
		f32 rotation[3];
		f32 scale[3];

		transform::init_info to_init_info()
		{
			using namespace DirectX;
			transform::init_info info{};
			memcpy(&info.position[0], &position[0], sizeof(position));
			memcpy(&info.scale[0], &scale[0], sizeof(scale));

			XMFLOAT3A rot{ &rotation[0] };
			XMVECTOR quat{ XMQuaternionRotationRollPitchYawFromVector(XMLoadFloat3A(&rot)) };
			XMFLOAT4A rot_quat{};
			XMStoreFloat4A(&rot_quat, quat);

			memcpy(&info.rotation[0], &rot_quat.x, sizeof(info.rotation));

			return info;
		}
	};

	struct script_component
	{
		script::detail::script_creator script_creator;

		script::init_info to_init_info()
		{
			script::init_info info{};
			info.script_creator = script_creator;

			return info;
		}
	};

	struct entity_description
	{
		transform_component transform;
		script_component script;
	};

	entity::entity entity_from_id(id::id_type id)
	{
		return entity::entity{ entity::entity_id{ id } };
	}
}

EDITOR_INTERFACE
id::id_type CreateEntity(entity_description* e)
{
	assert(e);
	entity_description& desc{ *e };
	transform::init_info transform_info{ desc.transform.to_init_info() };
	script::init_info script_info{ desc.script.to_init_info() };

	entity::entity_info entity_info
	{
		.transform = &transform_info,
		.script = &script_info,
	};

	return entity::create(entity_info).get_id();
}

EDITOR_INTERFACE
void RemoveEntity(id::id_type id)
{
	assert(id::is_valid(id));

	entity::remove(entity::entity_id{ id });
}