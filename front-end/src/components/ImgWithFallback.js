import React, { useState } from "react";

function ImgWithFallback({ src, alt, className }) {
  const [imgError, setImgError] = useState(false);

  if (!src || imgError) {
    return (
      <div className={className + " flex items-center justify-center bg-gray-200"}>
        <svg
          width="64"
          height="64"
          viewBox="0 0 64 64"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
          className="text-gray-400"
        >
          <rect width="64" height="64" rx="8" fill="#e5e7eb"/>
          <path d="M16 44L28 32L36 40L44 32L56 44" stroke="#9ca3af" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
          <circle cx="20" cy="24" r="4" fill="#9ca3af"/>
        </svg>
      </div>
    );
  }

  return (
    <img
      src={src}
      alt={alt}
      className={className}
      onError={() => setImgError(true)}
    />
  );
}

export default ImgWithFallback;
