#include "Transform.h"
#include "Entity.h"

namespace fabric::transform
{
	namespace
	{
		utl::vector<math::vec3> positions;
		utl::vector<math::quat> rotations;
		utl::vector<math::vec3> scales;
	}

	component create(const init_info& info, entity::entity entity)
	{
		assert(entity.is_valid());
		const id::id_type entity_index{ id::index(entity.get_id()) };

		if (positions.size() > entity_index)
		{
			positions[entity_index] = math::vec3(info.position);
			rotations[entity_index] = math::quat(info.rotation);
			scales[entity_index] = math::vec3(info.scale);
		}
		else
		{
			assert(positions.size() == entity_index);
			positions.emplace_back(info.position);
			rotations.emplace_back(info.rotation);
			scales.emplace_back(info.scale);
		}

		return component(transform_id{ id::id_type(positions.size() - 1) });
	}

	void remove(component c)
	{
		assert(c.is_valid());
	}

	math::vec3 component::position() const
	{
		assert(is_valid());

		return positions[id::index(_id)];
	}

	math::quat component::rotation() const
	{
		assert(is_valid());

		return rotations[id::index(_id)];
	}

	math::vec3 component::scale() const
	{
		assert(is_valid());

		return scales[id::index(_id)];
	}
}