#pragma once

#include "EngineAPI/Components.h"

using namespace fabric;

class TestScript : public component::script
{
public:
	constexpr TestScript(entity::entity entity) : script(entity) {}

	virtual void create() override;
	virtual void update(float dt) override;
	virtual void destroy() override;

private:

};

