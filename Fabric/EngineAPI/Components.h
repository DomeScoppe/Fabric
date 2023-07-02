#pragma once

#include "../Utilities/Math.h"
#include "EngineAPI/GameEntity.h"
#include "../GameEntity/Entity.h"
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

		u8 register_script(entity::component_id id, script_creator func);

#ifdef USE_WITH_EDITOR
		extern "C" __declspec(dllexport)
#endif
			script_creator get_script_creator(entity::component_id id);

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
		const u8 _reg_##TYPE = fabric::component::detail::register_script(fabric::entity::get_component_id<TYPE>(),\
																			&fabric::component::detail::create_script<TYPE>);\
		const u8 _name_##TYPE = fabric::component::detail::add_script_name(#TYPE);\
	}
#else
#define REGISTER_SCRIPT(TYPE)\
namespace\
	{\
		const u8 _reg_##TYPE = fabric::component::detail::register_script(fabric::entity::get_component_id<TYPE>(),\
																			&fabric::component::detail::create_script<TYPE>);\
	}
#endif
}

namespace fabric::entity
{
	template<typename T> requires IsComponent<T>
	const component_id get_component_id()
	{
		static component_id id = component_id(id::index_pair(_componentCounter++));
		return id;
	}

	template<typename T> requires IsScript<T>
	const component_id get_component_id()
	{
		static component_id id = component_id(id::index_pair(id::pair_value1(get_component_id<component::script>()), _componentCounter++));
		return id;
	}

	template<typename T> requires IsScriptComponent<T>
	const component_id get_component_id()
	{
		static component_id id = component_id(_componentCounter++);
		return id;
	}
}