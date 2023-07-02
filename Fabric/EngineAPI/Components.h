#pragma once

#include "../Utilities/Math.h"
#include "EngineAPI/GameEntity.h"
#include "EngineAPI/Scene.h"

namespace fabric::component
{
	struct transform
	{
	public:
		math::vec3 position;
		math::quat rotation;
		math::vec3 scale;

		constexpr transform(const math::vec3& pos, const math::quat& rot, const math::vec3& sc)
			: position{ pos }, rotation{ rot }, scale{ sc } {}
		transform(const math::vec3& pos, const math::vec3& rot, const math::vec3& sc)
			: position{ pos }, scale{ sc }
		{
			using namespace DirectX;
			XMVECTOR quaternion = XMQuaternionRotationRollPitchYawFromVector(XMLoadFloat3(&rot));
			XMStoreFloat4(&rotation, quaternion);
		}
	};

	struct script
	{
	public:
		virtual ~script() = default;

		constexpr entity::entity get_entity() { return entity::entity(_id); }

		virtual void create() {};
		virtual void update(float dt) {};
		virtual void destroy() {};

	protected:
		constexpr explicit script(entity::entity entity) : _id{ entity.get_id()} {}

	private:
		entity::entity_id _id;
	};

	namespace detail
	{
		using script_ptr = std::unique_ptr<script>;
		using script_creator = script_ptr(*)(entity::entity_id);
		using script_hash = std::hash<std::string>;

		u8 register_script(size_t tag, script_creator func);

#ifdef USE_WITH_EDITOR
		extern "C" __declspec(dllexport)
#endif
			script_creator get_script_creator(size_t tag);

		template<typename script_class>
		script_ptr create_script(entity::entity_id id)
		{
			assert(scene::is_alive(id));

			return std::make_unique<script_class>(entity::entity(id));
	}

#ifdef USE_WITH_EDITOR
		u8 add_script_name(const char* name);
#endif
	}

#ifdef USE_WITH_EDITOR
#define REGISTER_SCRIPT(TYPE)\
namespace\
	{\
		const u8 _reg_##TYPE = fabric::component::detail::register_script(fabric::component::detail::script_hash()(#TYPE),\
																			&fabric::component::detail::create_script<TYPE>);\
		const u8 _name_##TYPE = fabric::component::detail::add_script_name(#TYPE);\
	}
#else
#define REGISTER_SCRIPT(TYPE)\
namespace\
	{\
		const u8 _reg_##TYPE = fabric::component::detail::register_script(fabric::component::detail::script_hash()(#TYPE),\
																			&fabric::component::detail::create_script<TYPE>);\
	}
#endif
}