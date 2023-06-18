#pragma once

#include "Common.h"

namespace fabric::transform
{
	struct init_info
	{
		f32 position[3]{};
		f32 rotation[4]{};
		f32 scale[3]{ 1.0f, 1.0f, 1.0f };
	};

	component create(const init_info& info, entity::entity entity);
	void remove(component c);
}