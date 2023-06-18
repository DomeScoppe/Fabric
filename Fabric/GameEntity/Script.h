#pragma once

#include "Common.h"

namespace fabric::script
{
	struct init_info
	{
		detail::script_creator script_creator;
	};

	component create(const init_info& info, entity::entity entity);
	void remove(component c);
	void update(float dt);
}