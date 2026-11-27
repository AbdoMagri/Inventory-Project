export default function Input({ label, error, hint, icon, ...props }) {
  return (
    <div className="field">
      {label && <label>{label}</label>}
      <div className="input-wrap">
        {icon && <span className="input-icon">{icon}</span>}
        <input
          className={`input ${icon ? "input-with-icon" : ""} ${error ? "input-error" : ""}`}
          {...props}
        />
      </div>
      {error ? (
        <span className="field-error">{error}</span>
      ) : (
        hint && <span className="field-hint">{hint}</span>
      )}
    </div>
  );
}