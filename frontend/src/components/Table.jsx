import { useState } from "react";

export default function Table({
  columns,
  data,
  selectable = false,
  onRowClick,
  emptyText = "No data",
}) {
  const [selected, setSelected] = useState([]);
  const [sort, setSort] = useState({ key: null, dir: "asc" });

  const toggleSort = (c) => {
    if (!c.sortable) return;
    setSort((s) => ({
      key: c.key,
      dir: s.key === c.key && s.dir === "asc" ? "desc" : "asc",
    }));
  };

  const rows = sort.key
    ? [...data].sort((a, b) => {
        const x = a[sort.key], y = b[sort.key];
        const r = x > y ? 1 : x < y ? -1 : 0;
        return sort.dir === "asc" ? r : -r;
      })
    : data;

  const allChecked = rows.length > 0 && selected.length === rows.length;
  const toggleAll = () => setSelected(allChecked ? [] : rows.map((r) => r.id));
  const toggleOne = (id) =>
    setSelected((s) => (s.includes(id) ? s.filter((x) => x !== id) : [...s, id]));

  const colCount = columns.length + (selectable ? 1 : 0);

  return (
    <table className="table">
      <thead>
        <tr>
          {selectable && (
            <th className="col-check">
              <input type="checkbox" checked={allChecked} onChange={toggleAll} />
            </th>
          )}
          {columns.map((c) => (
            <th
              key={c.key}
              className={c.sortable ? "sortable" : ""}
              onClick={() => toggleSort(c)}
            >
              {c.title}
              {c.sortable && (
                <span className="sort-icon">
                  {sort.key === c.key ? (sort.dir === "asc" ? "↑" : "↓") : "↕"}
                </span>
              )}
            </th>
          ))}
        </tr>
      </thead>
      <tbody>
        {rows.length === 0 ? (
          <tr>
            <td colSpan={colCount} className="table-empty">{emptyText}</td>
          </tr>
        ) : (
          rows.map((row, i) => (
            <tr
              key={row.id ?? i}
              className={onRowClick ? "row-click" : ""}
              onClick={() => onRowClick?.(row)}
            >
              {selectable && (
                <td className="col-check" onClick={(e) => e.stopPropagation()}>
                  <input
                    type="checkbox"
                    checked={selected.includes(row.id)}
                    onChange={() => toggleOne(row.id)}
                  />
                </td>
              )}
              {columns.map((c) => (
                <td key={c.key}>{c.render ? c.render(row) : row[c.key]}</td>
              ))}
            </tr>
          ))
        )}
      </tbody>
    </table>
  );
}