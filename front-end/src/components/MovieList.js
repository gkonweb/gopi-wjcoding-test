import React from "react";
import MovieCard from "./MovieCard";

function MovieList({ movies, onPriceClick }) {
  if (!movies.length) return <div className="text-gray-500 text-center py-8">No movies found.</div>;
  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-8">
      {movies.map((movie) => (
        <MovieCard key={movie.commonID} movie={movie} onPriceClick={onPriceClick} />
      ))}
    </div>
  );
}

export default MovieList;
