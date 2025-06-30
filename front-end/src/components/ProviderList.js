import React from "react";

function ProviderList({ prices, onPriceClick }) {
  if (!prices.length)
    return <div className="text-gray-400">No providers available.</div>;
  const sorted = [...prices].sort((a, b) =>
    a.isCheapest === b.isCheapest ? 0 : a.isCheapest ? -1 : 1
  );
  return (
    <div className="w-full">
      <strong className="block mb-1 text-gray-700">Prices:</strong>
      <ul className="space-y-1">
        {sorted.map((price) => (
          <li key={price.id}>
            <a
              href="#"
              className={`transition-colors ${
                price.isCheapest
                  ? "text-green-600 font-bold underline"
                  : "text-blue-600 hover:underline"
              }`}
              onClick={(e) => {
                e.preventDefault();
                onPriceClick(price.id);
              }}
            >
              {price.source}: ${price.price}
              {price.isCheapest && " (lowest)"}
            </a>
          </li>
        ))}
      </ul>
    </div>
  );
}

export default ProviderList;
