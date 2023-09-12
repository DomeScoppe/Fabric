#pragma once

namespace fabric::utl
{
	template<typename... Types>
	struct type_list {};

	template<typename List>
	struct front;

	template<typename Head, typename... Tail>
	struct front<type_list<Head, Tail...>>
	{
		using Type = Head;
	};

	template<typename List>
	using front_t = typename front<List>::Type;
}