-------------------------------------------------------------------------
-- PKG_PLATFORMS
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_platforms IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_platforms (
        po_cursor   OUT sys_refcursor
    );

END pkg_platforms;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_platforms IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_platforms (
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                platform_id,
                platform_name
            from
                platforms
            where
                deleted is null;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_all_platforms' || chr(10) || sqlerrm);
    END p_get_all_platforms;

END pkg_platforms;
/
-------------------------------------------------------------------------