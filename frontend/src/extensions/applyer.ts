import type { RouteObject } from "react-router-dom";
import { PatchableRouteObject } from "./extension";

// Функция собирает новое дерево роутов: для каждого узла из extra либо вставляет его в корень,
// либо обновляет существующий узел, не затирая непереданные поля.
// Дочерние маршруты мерджатся по id, а normalizeRoute следит, чтобы у индексных роутов не было path.
export function ApplyExtensions(
  base: PatchableRouteObject[],
  extra: PatchableRouteObject[]
): RouteObject[] {
  let result = [...base];

  for (const ext of extra) {
    const idx = result.findIndex((r) => r.id && r.id === ext.id);

    if (idx === -1) {
      // Не было — добавляем как есть
      result = [...result, normalizeRoute(ext)];
    } else {
      // Был — мержим только переданные поля; детей мержим по id рекурсивно
      const merged = mergeRoute(result[idx], ext);
      result = [...result.slice(0, idx), merged, ...result.slice(idx + 1)];
    }
  }

  return result;
}

function mergeRoute(
  base: PatchableRouteObject,
  ext: PatchableRouteObject
): PatchableRouteObject {
  const mergedTop: PatchableRouteObject = {
    // перезаписываем только то, что прислали
    element: ext.element ?? base.element,
    path: ext.path ?? base.path,
    index: ext.index ?? base.index,
    loader: ext.loader ?? base.loader,
    action: ext.action ?? base.action,
    errorElement: ext.errorElement ?? base.errorElement,
    handle: ext.handle ?? base.handle,
    shouldRevalidate: ext.shouldRevalidate ?? base.shouldRevalidate,
    id: base.id,
  };

  // Если это index-роут, у него не должно быть path
  if (mergedTop.index === true) mergedTop.path = undefined;

  const baseChildren = base.children ?? [];
  const extChildren = ext.children ?? [];

  if (extChildren.length === 0) {
    // Нечего мержить — оставляем базовых детей
    mergedTop.children = baseChildren;
    return mergedTop;
  }

  const byId = new Map<string, PatchableRouteObject>();
  for (const c of baseChildren) if (c.id) byId.set(c.id, c);

  const out: PatchableRouteObject[] = [...baseChildren];

  for (const ec of extChildren) {
    if (ec.id && byId.has(ec.id)) {
      // Мержим существующего ребёнка
      const existing = byId.get(ec.id)!;
      const mergedChild = mergeRoute(existing, ec);
      const pos = out.findIndex((x) => x.id === ec.id);
      out[pos] = mergedChild;
    } else {
      // Ребёнка не было — добавляем
      out.push(normalizeRoute(ec));
    }
  }

  mergedTop.children = out;
  return mergedTop;
}

// Гарантируем корректность index/path
function normalizeRoute(r: PatchableRouteObject): PatchableRouteObject {
  const n: PatchableRouteObject = { ...r };
  if (n.index === true) n.path = undefined;
  if (n.children?.length) n.children = n.children.map(normalizeRoute);
  return n;
}
