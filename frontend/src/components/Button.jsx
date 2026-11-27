export default function Button({
  variant = "primary",
  size = "md",
  icon,
  children,
  className = "",
  ...props
}) {
  return (
    <button className={`btn btn-${variant} btn-${size} ${className}`} {...props}>
      {icon && <span className="btn-icon">{icon}</span>}
      {children}
    </button>
  );
}