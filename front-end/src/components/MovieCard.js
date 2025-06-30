import React from "react";
import ProviderList from "./ProviderList";
import ImgWithFallback from "./ImgWithFallback";

function MovieCard({ movie, onPriceClick }) {
  return (
    <div className="bg-white rounded-lg shadow p-6 flex flex-col items-center">
      <h2 className="text-xl font-semibold mb-2 text-blue-800">{movie.title}</h2>
      <ImgWithFallback
        src={movie.poster}
        alt={movie.title}
        className="w-32 h-48 object-cover rounded mb-4 shadow"
      />
      <ProviderList prices={movie.prices} onPriceClick={onPriceClick} />
    </div>
  );
}

export default MovieCard;
