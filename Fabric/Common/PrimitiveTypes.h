#pragma once

#include <stdint.h>

// unsigned integers

using u8  = uint8_t;
using u16 = uint16_t;
using u32 = uint32_t;
using u64 = uint64_t;

// signed integers

using i8  =	int8_t;
using i16 = int16_t;
using i32 = int32_t;
using i64 = int64_t;

using f32 = float;

constexpr u64 u64_invalid_id{ 0xffff'ffff'ffff'ffffUi64 };
constexpr u32 u32_invalid_id{ 0xffff'ffffU };
constexpr u16 u16_invalid_id{ 0xffffU };
constexpr u8  u8_invalid_id { 0xffU };
